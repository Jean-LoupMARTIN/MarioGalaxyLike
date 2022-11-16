using UnityEngine;



public class GravitySphere : GravityField
{
    [SerializeField] float radius = 10;
    float radius2;
    [SerializeField] bool drawGizmo = true;
    [SerializeField] Color gizmoColor = Color.red;

    void OnDrawGizmos()
    {
        if (drawGizmo)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

    void OnValidate()
    {
        Cache();
    }

    void Awake()
    {
        Cache();
    }

    void Cache()
    {
        radius2 = radius * radius;
    }


    protected override (Vector3 dir, float dist) AtPos(Vector3 pos)
    {
        Vector3 posToCenter = transform.position - pos;
        return (posToCenter.normalized, Mathf.Max(0, posToCenter.magnitude - radius));
    }
}