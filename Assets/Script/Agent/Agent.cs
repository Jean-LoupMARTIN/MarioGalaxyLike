using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(MoveWithRef))]
public class Agent : MonoBehaviour
{
    [SerializeField] Controller controller;

    [Space(10), Header("Physic"), Space(5)]
    [SerializeField] float gravityAcce = 40;
    [SerializeField] float dragGround = 5;
    [SerializeField] float dragAir = 1;
    (Vector3 dir, float dist, GravityField field) gravity = (Vector3.zero, float.MaxValue, null);
    bool fieldHasChanged = false;

    [Space(10), Header("Ground"), Space(5)]
    [SerializeField] float agentHeight = 1f;
    float agentHeight2; // agentHeight / 2
    [SerializeField] float checkGroundRayMarge = 0.2f;
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
    [HideInInspector] public UnityEvent<Transform> OnSetMoveWithRefTransform = new UnityEvent<Transform>();


    public bool IsPlayer => controller == InputController.inst;
    public bool IsFocused => PlayerCamera.inst.Agent == this && CameraExtension.current == PlayerCamera.inst.Cam;

    public Controller Controller { get => controller; }
    public float Height { get => agentHeight; }
    public float Height2 { get => agentHeight2; }
    public Vector3 Velocity { get => rb.velocity; set => rb.velocity = value; }
    public Vector3 LocalVelocity { get => transform.InverseTransformDirection(Velocity); set => Velocity = transform.TransformDirection(value); }
    public bool Grounded { get => grounded; }
    public Rigidbody Rigidbody { get => rb; }
    public float GravityAcce { get => gravityAcce; }
    public float DragAir { get => dragAir; }
    public float CancelCheckGroundTime { set => cancelCheckGroundTime = value; }


    public void SetGrounded(bool grounded)
    {
        if (this.grounded == grounded)
            return;

        this.grounded = grounded;
        rb.drag = grounded ? dragGround : dragAir;
    }


    public void SetGroundCollider(Collider collider)
    {
        if (groundCollider == collider)
            return;

        groundCollider = collider;
        UpdateMoveWithRefTransfrom();
    }

    void SetGravity((Vector3 dir, float dist, GravityField field) gravity)
    {
        fieldHasChanged = gravity.field != this.gravity.field;
        this.gravity = gravity;
        UpdateMoveWithRefTransfrom();
    }

    bool CancelCheckGround => Time.time < cancelCheckGroundTime;





    void OnDrawGizmosSelected()
    {
        if (drawCheckGroundRay) DrawCheckGroundRay();
        if (drawGroundCollider) DrawGroundCollider();
    }

    void DrawCheckGroundRay()
    {
        Gizmos.color = checkGroundRayColor;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * (agentHeight / 2 + checkGroundRayMarge));
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

        agentHeight2 = agentHeight / 2;
    }


    void FixedUpdate()
    {
        CacheGravity();

        if (!CancelCheckGround)
            CheckGround();

        Vector3 localVelocity = Vector3.zero;

        // if gravity field has not chaged since the last frame
        // -> get local velocity before RotationMatchGravity
        if (!fieldHasChanged) localVelocity = LocalVelocity;

        RotationMatchGravity();

        // else -> get local velocity after RotationMatchGravity
        if (fieldHasChanged) localVelocity = LocalVelocity;

        if (grounded) localVelocity.y = 0;
        LocalVelocity = localVelocity;

        if (!grounded) ApplyGravity();
    }


    void CacheGravity() => SetGravity(Gravity.AtPos(transform.position));

    void ApplyGravity() => rb.AddForce(gravityAcce * Gravity.Coef * gravity.dir, ForceMode.Acceleration);




    void RotationMatchGravity() => transform.MatchUp(-gravity.dir);

    void CheckGround()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, agentHeight2 + checkGroundRayMarge, groundLayer))
        {
            SetGrounded(true);
            SetGroundCollider(hit.collider);

            // match ground
            transform.position = hit.point + transform.up * agentHeight2;
        }

        else {
            SetGrounded(false);
            SetGroundCollider(null);
        }
    }

    void UpdateMoveWithRefTransfrom()
    {
        Transform trans;

        if      (grounded)      trans = groundCollider.transform;
        else if (gravity.field) trans = gravity.field.transform;
        else                    trans = null;

        moveWithRef.SetRefTransfrom(trans);
        OnSetMoveWithRefTransform.Invoke(trans);
    }
}
