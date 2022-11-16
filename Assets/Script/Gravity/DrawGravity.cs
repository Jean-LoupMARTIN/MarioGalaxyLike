using UnityEngine;




public class DrawGravity : MonoBehaviour
{
    [SerializeField] Vector3Int size = new Vector3Int(10, 10, 10);
    [SerializeField] float pointDistance = 3;
    [SerializeField] float pointRadius = 0.1f;
    [SerializeField] float arrowLength = 1;
    [SerializeField] Gradient gradient;
    [SerializeField] float distAtGradientMax = 10;

    Vector3 offset;

    void OnValidate()
    {
        offset = -(size - Vector3.one) / 2;
    }

    void OnDrawGizmos()
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                for (int z = 0; z < size.z; z++)
                    DrawGravityDir(transform.position + (new Vector3(x, y, z) + offset) * pointDistance);


    }
    void DrawGravityDir(Vector3 pos)
    {
        if (Application.isPlaying)
        {
            Vector3 dir;
            float dist;
            (dir, dist, _) = Gravity.AtPos(pos);
            Gizmos.color = gradient.Evaluate(Mathf.InverseLerp(0, distAtGradientMax, dist));
            GizmosExtension.DrawArrow(pos, pos + dir * arrowLength, arrowLength * 0.2f);
        }

        else {
            Gizmos.color = gradient.Evaluate(0);
            Gizmos.DrawSphere(pos, pointRadius);
        }
    }
}
