using UnityEngine;



[RequireComponent(typeof(Agent))]
public class AgentMove : MonoBehaviour
{
    [SerializeField] float acceGround = 100;
    [SerializeField] float acceAir = 10;

    Agent agent;

    void Awake()
    {
        agent = GetComponent<Agent>();
    }


    void FixedUpdate()
    {
        ApplyAcceleration();
    }

    void ApplyAcceleration()
    {
        Vector3 stickL3 = agent.Controller.StickL3;

        if (stickL3 != Vector3.zero)
        {
            float acce = agent.Grounded ? acceGround : acceAir;
            Vector3 dir;

            if (agent.IsPlayer)
            {
                Quaternion rot = TransformExtension.RotationMatchUp(CameraExtension.current.transform.rotation, transform.up);
                dir = rot * stickL3;
            }

            else dir = transform.rotation * stickL3;

            // apply force
            agent.Rigidbody.AddForce(dir * acce, ForceMode.Acceleration);

            // rotation match acceleration
            transform.rotation = Quaternion.LookRotation(dir, transform.up);
        }
    }
}
