using System.Collections.Generic;
using UnityEngine;




public static class PhysicsExtension
{
    static public bool ArcCast(Vector3 center, Quaternion rotation, float angle, float radius, int resolution, LayerMask layer, out RaycastHit hit, bool drawGizmo = false)
    {
        rotation *= Quaternion.Euler(-angle/2, 0, 0);

        float dAngle = angle / resolution;
        Vector3 forwardRadius = Vector3.forward * radius;

        Vector3 A, B, AB;
        A = forwardRadius;
        B = Quaternion.Euler(dAngle, 0, 0) * forwardRadius;
        AB = B - A;
        float AB_magnitude = AB.magnitude * 1.001f;

        for (int i = 0; i < resolution; i++)
        {
            A = center + rotation * forwardRadius;
            rotation *= Quaternion.Euler(dAngle, 0, 0);
            B = center + rotation * forwardRadius;
            AB = B - A;

            if (Physics.Raycast(A, AB, out hit, AB_magnitude, layer))
            {
                if (drawGizmo)
                    Gizmos.DrawLine(A, hit.point);

                return true;
            }

            if (drawGizmo)
                Gizmos.DrawLine(A, B);
        }

        hit = new RaycastHit();
        return false;
    }

    static public List<RaycastHit> RaycastSphere(Vector3 center, float radius, int nbRays, LayerMask layer, bool drawGizmo = false, float gizmoHitRadius = 0.1f)
        => RaycastSphere(center, 0, radius, nbRays, layer, drawGizmo, gizmoHitRadius);

    static public List<RaycastHit> RaycastSphere(Vector3 center, float radiusStart, float radiusEnd, int nbRays, LayerMask layer, bool drawGizmo = false, float gizmoHitRadius = 0.1f)
    {
        float turnFraction = 0.618f;
        float raycastLength = radiusEnd - radiusStart;
        List<RaycastHit> hits = new List<RaycastHit>();

        for (int i = 0; i < nbRays; i++)
        {
            float t = (float)i / (nbRays-1);
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = 2 * Mathf.PI * turnFraction * i;

            Vector3 dir = new Vector3(
                Mathf.Sin(inclination) * Mathf.Cos(azimuth),
                Mathf.Sin(inclination) * Mathf.Sin(azimuth),
                Mathf.Cos(inclination));

            Vector3 A = center + dir * radiusStart;
            Vector3 B = center + dir * radiusEnd;

            if (Physics.Raycast(A, dir, out RaycastHit hit, raycastLength, layer))
            {
                hits.Add(hit);

                if (drawGizmo)
                {
                    Gizmos.DrawLine(A, hit.point);
                    Gizmos.DrawSphere(hit.point, gizmoHitRadius);
                }
            }

            else if (drawGizmo)
                Gizmos.DrawLine(A, B);
        }

        return hits;
    }
}
