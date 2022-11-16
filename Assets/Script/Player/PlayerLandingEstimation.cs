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
            Estimate(player.LandingEstimationResult, true);
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

    public LandingEstimationResult Estimate(LandingEstimationResult lastResult = null, bool drawGizmo = false)
    {
        float t = 0;
        Vector3 pos = player.transform.position;
        Quaternion rot = player.transform.rotation;
        Vector3 velocity = player.Velocity;
        float lastResultTime = lastResult != null ? lastResult.time : 0;

        while (t < time)
        {
            float dt = Mathf.Min(Time.fixedDeltaTime, time - t);
            t += dt;

            // rotate with last landing estimation
            if (lastResult != null)
            {
                float landingProgress = Mathf.Clamp01(dt / lastResultTime);
                lastResultTime -= dt;
                Quaternion landingRot = TransformExtension.RotationMatchUp(rot, -Gravity.AtPos(pos).dir);
                rot = Quaternion.Lerp(rot, landingRot, landingProgress);
            }

            // rotate
            Vector3 localVelocity = Quaternion.Inverse(rot) * velocity;
            rot *= Quaternion.Euler(0, player.RotSpeedAir * dt * controller.StickR.x, 0);
            velocity = rot * localVelocity;

            // apply gravity
            velocity += Gravity.AcceAtPos(pos) * dt;

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