using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerLandingEstimation : MonoBehaviour
{
    [SerializeField] float time = 1;
    [SerializeField] LayerMask layer;
    [SerializeField] bool drawGizmo = true;
    [SerializeField] Color gizmoColor = Color.white;

    Player player;
    Controller controller;


    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && drawGizmo && !player.Grounded)
            Estimate(true);
    }


    void Awake()
    {
        Cache();        
    }

    void Cache()
    {
        player = GetComponent<Player>();
        controller = player.Controller;
    }

    public LandingEstimationResult Estimate(bool drawGizmo = false)
    {
        float t = 0;
        Vector3 pos = player.transform.position;
        Quaternion rot = player.transform.rotation;
        Vector3 velocity = player.Velocity;

        (Vector3 dir, float dist, GravityField field) gravity = Gravity.AtPos(pos);
        bool fieldHasChanged;

        while (t < time)
        {
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

            // rotate
            rot *= Quaternion.Euler(0, player.RotSpeedAir * dt * controller.StickR.x, 0);

            velocity = rot * localVelocity;

            // apply gravity
            velocity += Gravity.inst.Acceleration * dt * gravity.dir;

            // apply acceleration
            velocity += rot * controller.StickL3 * player.AcceAir * dt;

            // apply drag
            velocity *= 1 - player.DragAir * dt;

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
            if (Physics.CheckSphere(pos, player.Radius, layer))
            {
                if (drawGizmo)
                    Gizmos.DrawSphere(pos, player.Radius);

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