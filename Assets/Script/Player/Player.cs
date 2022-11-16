using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(MoveWithRef), typeof(PlayerLandingEstimation))]
public class Player : MonoBehaviour
{
    [SerializeField] Controller controller;

    [Space(10), Header("Physic"), Space(5)]
    [SerializeField] float acceGround = 10;
    [SerializeField] float acceAir = 10;
    [Space(10)]
    [SerializeField] float dragGround = 5;
    [SerializeField] float dragAir = 1;
    [Space(10)]
    [SerializeField] float rotSpeedGround = 180;
    [SerializeField] float rotSpeedAir = 180;
    (Vector3 dir, float dist, GravityField field) gravity = (Vector3.zero, float.MaxValue, null);

    [Space(10), Header("Jump"), Space(5)]
    [SerializeField] float jumpVelocityY = 20;
    [SerializeField] float jumpVelocityXZ = 20;
    [SerializeField] float cancelCheckGroundAfterJump = 0.5f;
    float lastJumpTime = -1000;

    [Space(10), Header("Ground"), Space(5)]
    [SerializeField] float playerRadius = 0.25f;
    [SerializeField] float checkGroundRayMarge = 0.1f;
    [SerializeField] LayerMask groundLayer;
    bool grounded;
    float cancelCheckGroundTime = -1000;

    [Space(10), Header("Gizmo"), Space(5)]
    [SerializeField] bool drawCheckGroundRay = true;
    [SerializeField] Color checkGroundRayColor = Color.yellow;
    [SerializeField] bool drawGroundCollider = true;
    [SerializeField] Color groundColliderColor = Color.green;
    Collider groundCollider;

    Rigidbody rb;
    MoveWithRef moveWithRef;
    PlayerLandingEstimation landingEstimation;
    LandingEstimationResult landingEstimationResult = null;



    public Controller Controller { get => controller; }
    public float Radius { get => playerRadius; }
    public Vector3 Velocity { get => rb.velocity; set => rb.velocity = value; }
    public Vector3 LocalVelocity { get => transform.InverseTransformDirection(Velocity); set => Velocity = transform.TransformDirection(value); }
    public bool Grounded { get => grounded; }
    public LandingEstimationResult LandingEstimationResult { get => landingEstimationResult; }
    public float AcceAir { get => acceAir; }
    public float DragAir { get => dragAir; }
    public float RotSpeedAir { get => rotSpeedAir; }


    void SetGrounded(bool grounded)
    {
        if (this.grounded == grounded)
            return;

        this.grounded = grounded;
        rb.drag = grounded ? dragGround : dragAir;
        landingEstimationResult = null;
    }


    void SetGroundCollider(Collider collider)
    {
        if (groundCollider == collider)
            return;

        groundCollider = collider;
        UpdateMoveWithRefTransfrom();
    }

    void SetGravity((Vector3 dir, float dist, GravityField field) gravity)
    {
        this.gravity = gravity;
        UpdateMoveWithRefTransfrom();
    }

    float TimeSinceLastJump => Time.time - lastJumpTime;
    bool CancelCheckGround => Time.time < cancelCheckGroundTime;





    void OnDrawGizmosSelected()
    {
        if (drawCheckGroundRay) DrawCheckGroundRay();
        if (drawGroundCollider) DrawGroundCollider();
    }

    void DrawCheckGroundRay()
    {
        Gizmos.color = checkGroundRayColor;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * (playerRadius + checkGroundRayMarge));
    }

    void DrawGroundCollider()
    {
        if (!groundCollider)
            return;

        Gizmos.color = groundColliderColor;
        GizmosExtension.DrawCollider(groundCollider);
    }

    void Awake()
    {
        Cache();
        CheckGround();
    }

    void Cache()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.angularDrag = 0;
        rb.drag = grounded ? dragGround : dragAir;

        moveWithRef = GetComponent<MoveWithRef>();
        landingEstimation = GetComponent<PlayerLandingEstimation>();
    }

    void OnEnable()
    {
        controller.X.OnPressDown.AddListener(Jump);
    }

    void OnDisable()
    {
        controller.X.OnPressDown.RemoveListener(Jump);
    }


    void FixedUpdate()
    {
        UpdateGravity();

        if (!CancelCheckGround)
            CheckGround();


        if (grounded)
        {
            Vector3 localVelocity = LocalVelocity;
            localVelocity.y = 0;
            RotationMatchGravity();
            LocalVelocity = localVelocity;
        }

        else RotateWithLandingEstimation();

        Rotate();

        if (!grounded) ApplyGravity();
        ApplyAcceleration();
    }

    void UpdateGravity() => SetGravity(Gravity.AtPos(transform.position));

    void ApplyGravity() => rb.AddForce(gravity.dir * Gravity.inst.Acceleration, ForceMode.Acceleration);

    void ApplyAcceleration()
    {
        Vector3 stickL3 = controller.StickL3;

        if (stickL3 != Vector3.zero)
            rb.AddRelativeForce(stickL3 * (grounded ? acceGround : acceAir), ForceMode.Acceleration);
    }

    void Rotate()
    {
        Vector3 localVelocity = LocalVelocity;
        transform.Rotate(0, (grounded ? rotSpeedGround : rotSpeedAir) * Time.fixedDeltaTime * controller.StickR.x, 0);
        LocalVelocity = localVelocity;
    }

    void RotationMatchGravity() => transform.MatchUp(-gravity.dir);

    void RotateWithLandingEstimation()
    {
        landingEstimationResult = landingEstimation.Estimate(landingEstimationResult);
        float landingProgress = Mathf.Clamp01(Time.fixedDeltaTime / landingEstimationResult.time);
        Quaternion landingRot = TransformExtension.RotationMatchUp(transform.rotation, -Gravity.AtPos(landingEstimationResult.pos).dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, landingRot, landingProgress);
    }


    void CheckGround()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, playerRadius + checkGroundRayMarge, groundLayer))
        {
            SetGrounded(true);
            SetGroundCollider(hit.collider);

            // match ground
            transform.position = hit.point + transform.up * playerRadius;
        }

        else {
            SetGrounded(false);
            SetGroundCollider(null);
        }
    }


    void Jump()
    {
        if (grounded) GroundJump();
        else AirJump();
    }

    void GroundJump()
    {
        Vector3 localVelocity = LocalVelocity;
        localVelocity.y = jumpVelocityY;
        LocalVelocity = localVelocity;

        SetGrounded(false);
        SetGroundCollider(null);
        cancelCheckGroundTime = Time.time + cancelCheckGroundAfterJump;

        lastJumpTime = Time.time;
    }

    void AirJump()
    {
        LocalVelocity = jumpVelocityY * Vector3.up + jumpVelocityXZ * controller.StickL3;
        cancelCheckGroundTime = Time.time + cancelCheckGroundAfterJump;
        lastJumpTime = Time.time;
    }

    void UpdateMoveWithRefTransfrom()
    {
        if      (grounded)      moveWithRef.SetRefTransfrom(groundCollider.transform);
        else if (gravity.field) moveWithRef.SetRefTransfrom(gravity.field.transform);
        else                    moveWithRef.SetRefTransfrom(null);
    }
}
