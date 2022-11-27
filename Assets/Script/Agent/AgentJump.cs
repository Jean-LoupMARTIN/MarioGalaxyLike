using UnityEngine;


[RequireComponent(typeof(Agent))]
public class AgentJump : MonoBehaviour
{
    [SerializeField] float jumpVelocityY = 15;
    [SerializeField] float jumpVelocityXZ = 7.5f;
    [SerializeField] float cancelCheckGroundAfterJump = 0.25f;
    [SerializeField] AudioClip sound;
    float lastJumpTime = -1000;

    float TimeSinceLastJump => Time.time - lastJumpTime;

    Agent agent;

    void Awake()
    {
        agent = GetComponent<Agent>();
    }

    void OnEnable()
    {
        agent.Controller.X.OnPressDown.AddListener(Jump);
    }

    void OnDisable()
    {
        agent.Controller.X.OnPressDown.RemoveListener(Jump);
    }

    void Jump()
    {
        if (agent.Grounded) GroundJump();
        else AirJump();

        // play sound
        AudioSourceExtension.PlayClipAtPoint(sound, transform.position);

        // cancel CheckGround
        agent.CancelCheckGroundTime = Time.time + cancelCheckGroundAfterJump;

        lastJumpTime = Time.time;
    }

    void GroundJump()
    {
        Vector3 localVelocity = agent.LocalVelocity;
        localVelocity.y = jumpVelocityY;
        agent.LocalVelocity = localVelocity;

        agent.SetGrounded(false);
        agent.SetGroundCollider(null);

    }

    void AirJump()
    {
        if (agent.IsPlayer)
        {
            Quaternion camRotMatchUp = TransformExtension.RotationMatchUp(CameraExtension.current.transform.rotation, transform.up);
            Quaternion dRot = transform.WorldToLocal(camRotMatchUp);
            agent.LocalVelocity = jumpVelocityY * Vector3.up + dRot * agent.Controller.StickL3 * jumpVelocityXZ;
        }

        else agent.LocalVelocity = jumpVelocityY * Vector3.up + agent.Controller.StickL3 * jumpVelocityXZ;
    }
}
