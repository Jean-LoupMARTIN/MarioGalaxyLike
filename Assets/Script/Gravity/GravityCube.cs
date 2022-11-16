using UnityEngine;



public class GravityCube : GravityField
{
    [SerializeField] Vector3 size = new Vector3(10, 10, 10);
    [SerializeField] float marge = 0.01f;
    Vector3 size2; // size / 2
    Vector3 size2Marge; // size2 + marge
    [SerializeField] bool drawGizmo = true;
    [SerializeField] Color gizmoColor = Color.red;

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
        size2 = size / 2;
        size2Marge = size2 + Vector3.one * marge;
    }

    void OnDrawGizmos()
    {
        if (drawGizmo)
        {
            Gizmos.color = gizmoColor;
            GizmosExtension.DrawCube(transform.position, transform.rotation, size, true);
            GizmosExtension.DrawCube(transform.position, transform.rotation, size + 2 * marge * Vector3.one, true);
        }
    }


    protected override (Vector3 dir, float dist) AtPos(Vector3 pos)
    {
        Vector3 localPos = transform.WorldToLocal(pos);

        // inside cube
        if (localPos.x >= -size2Marge.x && localPos.x <= size2Marge.x &&
            localPos.y >= -size2Marge.y && localPos.y <= size2Marge.y &&
            localPos.z >= -size2Marge.z && localPos.z <= size2Marge.z)
        {
            float distR = Mathf.Abs(localPos.x - size2Marge.x);
            float distL = Mathf.Abs(localPos.x + size2Marge.x);
            float distU = Mathf.Abs(localPos.y - size2Marge.y);
            float distD = Mathf.Abs(localPos.y + size2Marge.y);
            float distF = Mathf.Abs(localPos.z - size2Marge.z);
            float distB = Mathf.Abs(localPos.z + size2Marge.z);

            float distMin = Mathf.Min(distR, distL, distU, distD, distF, distB);

            Vector3 localDir;

            if      (distMin == distR) localDir = Vector3.left;
            else if (distMin == distL) localDir = Vector3.right;
            else if (distMin == distU) localDir = Vector3.down;
            else if (distMin == distD) localDir = Vector3.up;
            else if (distMin == distF) localDir = Vector3.back;
            else                       localDir = Vector3.forward;

            return (transform.TransformDirection(localDir), 0);
        }

        // outside cube
        else {
            localPos.x = Mathf.Clamp(localPos.x, -size2.x, size2.x);
            localPos.y = Mathf.Clamp(localPos.y, -size2.y, size2.y);
            localPos.z = Mathf.Clamp(localPos.z, -size2.z, size2.z);

            Vector3 worldPos = transform.LocalToWorld(localPos);

            Vector3 dir = worldPos - pos;
            float dist = dir.magnitude;
            if (dist != 0) dir /= dist;

            return (dir, dist);
        }
    }
}


