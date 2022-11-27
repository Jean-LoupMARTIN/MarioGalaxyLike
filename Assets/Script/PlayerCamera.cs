using UnityEngine;



[RequireComponent(typeof(MoveWithRef))]
public class PlayerCamera : MonoBehaviour
{
    static public PlayerCamera inst;

    [SerializeField] Transform target;
    [SerializeField] float rotSpeed = 180;

    Vector3 posOffset;
    Quaternion rotOffset;
    Agent agent;
    Controller controller;
    Camera cam;
    MoveWithRef moveWithRef;

    public Agent Agent { get => agent; }
    public Camera Cam { get => cam; }

    void Awake()
    {
        inst = this;
        moveWithRef = GetComponent<MoveWithRef>();
        agent = target.GetComponentInParent<Agent>();
        agent?.OnSetMoveWithRefTransform.AddListener(moveWithRef.SetRefTransfrom);
        controller = agent?.Controller;
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        Quaternion rotMatchTargetUp = TransformExtension.RotationMatchUp(transform.rotation, target.up);
        rotOffset = Quaternion.Inverse(rotMatchTargetUp) * transform.rotation;
        posOffset = -transform.WorldToLocal(target.position);
    }

    void FixedUpdate()
    {
        Quaternion rotMatchTargetUp = TransformExtension.RotationMatchUp(transform.rotation, target.up);
        transform.rotation = rotMatchTargetUp * rotOffset;
        transform.position = target.position + transform.rotation * posOffset;

        if (controller)
            transform.RotateAround(target.position, target.up, rotSpeed * Time.fixedDeltaTime * controller.StickR.x);
    }
}
