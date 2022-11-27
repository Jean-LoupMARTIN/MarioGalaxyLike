using UnityEngine;


public class FollowTargetController : Controller
{
    [SerializeField] Transform target;
    [SerializeField] float distStop = 1;
    float distStop2; // distStop^2



    protected override void OnValidate()
    {
        base.OnValidate();
        Cache();
    }

    protected override void Awake()
    {
        base.Awake();
        Cache();
    }

    void Cache()
    {
        distStop2 = distStop * distStop;
    }
    
    void FixedUpdate()
    {
        if ((target.position - transform.position).sqrMagnitude < distStop2)
        {
            StickL = Vector2.zero;
            return;
        }

        Vector3 targetLocalPos = transform.WorldToLocal(target.position);

        Vector3 dir = targetLocalPos;
        dir.y = 0;
        if (dir != Vector3.zero)
            dir.Normalize();

        StickL = new Vector2(dir.x, dir.z);
    }
}
