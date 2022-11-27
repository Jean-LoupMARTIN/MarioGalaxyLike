using UnityEngine;


[RequireComponent(typeof(Agent))]
public class AgentLandingEstimation : MonoBehaviour
{
    [SerializeField] float time = 1;
    [SerializeField] LayerMask layer;
    [SerializeField] bool drawGizmo = true;
    [SerializeField] Color gizmoColor = Color.white;

    Agent agent;
    Controller controller;


    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && drawGizmo && !agent.Grounded)
            Estimate(true);
    }


    void Awake()
    {
        Cache();        
    }

    void Cache()
    {
        agent = GetComponent<Agent>();
        controller = agent.Controller;
    }

    public LandingEstimationResult Estimate(bool drawGizmo = false)
    {
        float t = 0;
        Vector3 pos = agent.transform.position;
        Quaternion rot = agent.transform.rotation;
        Vector3 velocity = agent.Velocity;

        (Vector3 dir, float dist, GravityField field) gravity = Gravity.AtPos(pos);
        bool fieldHasChanged;

        while (t < time)
        {
            // TODO move player camera

            float dt = Mathf.Min(Time.fixedDeltaTime, time - t);
            t += dt;

            // cache gravity
            (Vector3 dir, float dist, GravityField field) newGravity = Gravity.AtPos(pos);
            fieldHasChanged = newGravity.field != gravity.field;
            gravity = newGravity;

            Vector3 localVelocity = Vector3.zero;

            // if gravity field has not chaged since the last frame
            // -> get local velocity before RotationMatchGravity
            if (!fieldHasChanged) localVelocity = Quaternion.Inverse(rot) * velocity;

            // rotation match gravity dir;
            rot.MatchUp(-gravity.dir);

            // else -> get local velocity after RotationMatchGravity
            if (fieldHasChanged) localVelocity = Quaternion.Inverse(rot) * velocity;
            velocity = rot * localVelocity;

            // apply gravity
            velocity += agent.GravityAcce * Gravity.Coef * dt * gravity.dir;

            // TODO apply acceleration
            // velocity += rot * controller.StickL3 * player.AcceAir * dt;

            // apply drag
            velocity *= 1 - agent.DragAir * dt;

            // apply velocity
            Vector3 nextPos = pos + velocity * dt;

            // draw gizmo
            if (drawGizmo)
            {
                Color c = gizmoColor;
                c.a = 1 - Mathf.Clamp01(t / time);
                Gizmos.color = c;
                Gizmos.DrawLine(pos, nextPos);
            }

            pos = nextPos;

            // check collision
            if (Physics.CheckSphere(pos, agent.Height2, layer))
            {
                if (drawGizmo)
                    Gizmos.DrawSphere(pos, agent.Height2);

                break;
            }
        }

        return new LandingEstimationResult(pos, rot, t);
    }
}





public class LandingEstimationResult
{
    public Vector3 pos;
    public Quaternion rot;
    public float time;

    public LandingEstimationResult(Vector3 pos, Quaternion rot, float time)
    {
        this.pos = pos;
        this.rot = rot;
        this.time = time;
    }
}