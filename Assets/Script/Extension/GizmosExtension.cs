using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosExtension
{
    public static void DrawCircle(Vector3 pos, Vector3 forward, float radius, int resolution = 30)
    {
        Quaternion rot = Quaternion.LookRotation(forward);
        float drot = 360 / resolution;

        for (int i = 0; i < resolution; i++)
        {
            Vector3 from = pos + rot * Vector3.up * radius;
            rot *= Quaternion.Euler(Vector3.forward * drot);
            Vector3 to = pos + rot * Vector3.up * radius;

            Gizmos.DrawLine(from, to);
        }
    }

    public static void DrawWireCone(Vector3 pos, Vector3 forward, float length, float angle)
    {
        angle /= 2;
        Quaternion rot;

        rot = Quaternion.LookRotation(forward);
        rot *= Quaternion.Euler(Vector3.right * angle);
        Gizmos.DrawLine(pos, pos + rot * Vector3.forward * length);

        rot = Quaternion.LookRotation(forward);
        rot *= Quaternion.Euler(Vector3.left * angle);
        Gizmos.DrawLine(pos, pos + rot * Vector3.forward * length);

        rot = Quaternion.LookRotation(forward);
        rot *= Quaternion.Euler(Vector3.up * angle);
        Gizmos.DrawLine(pos, pos + rot * Vector3.forward * length);

        rot = Quaternion.LookRotation(forward);
        rot *= Quaternion.Euler(Vector3.down * angle);
        Gizmos.DrawLine(pos, pos + rot * Vector3.forward * length);

        angle = Mathf.Deg2Rad * angle;
        float adj = Mathf.Cos(angle) * length;
        float opp = Mathf.Sin(angle) * length;

        DrawCircle(pos + forward * adj, forward, opp);
    }

    public static void DrawArrow(Vector3 A, Vector3 B, float chevronLength, float chevronAngle = 90)
    {
        if (A == B)
            return;

        Gizmos.DrawLine(A, B);

        Vector3 BA = A - B;
        Vector3 camPos = Camera.current.transform.position;
        Vector3 toCam = B - camPos;

        Gizmos.DrawLine(B, B + Quaternion.LookRotation(BA, toCam) * Quaternion.Euler(0, chevronAngle / 2, 0) * Vector3.forward * chevronLength);
        Gizmos.DrawLine(B, B + Quaternion.LookRotation(BA, toCam) * Quaternion.Euler(0,-chevronAngle / 2, 0) * Vector3.forward * chevronLength);
    }


    public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 scale, bool wire = false)
    {
        Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, scale);
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        Gizmos.matrix *= cubeTransform;

        if (wire) Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        else      Gizmos.DrawCube    (Vector3.zero, Vector3.one);

        Gizmos.matrix = oldGizmosMatrix;
    }


    public static void DrawBoxCollider(BoxCollider bc, bool wire = false)
    {
        DrawCube(bc.transform.position + bc.transform.rotation * Vector3.Scale(bc.transform.lossyScale, bc.center),
                 bc.transform.rotation,
                 Vector3.Scale(bc.size, bc.transform.lossyScale),
                 wire);
    }


    public static void DrawSphereCollider(SphereCollider sc, bool wire = false)
    {
        Vector3 center = sc.transform.position + sc.transform.rotation * Vector3.Scale(sc.transform.lossyScale, sc.center);

        float radius = sc.radius * Mathf.Max(Mathf.Abs(sc.transform.lossyScale.x),
                                             Mathf.Abs(sc.transform.lossyScale.y),
                                             Mathf.Abs(sc.transform.lossyScale.z));

        if (wire) Gizmos.DrawWireSphere(center, radius);
        else      Gizmos.DrawSphere    (center, radius);
    }


    public static void DrawCollider(Collider c)
    {
        if (c.TryGetComponent(out MeshCollider mc))
            Gizmos.DrawMesh(mc.sharedMesh, mc.transform.position, mc.transform.rotation, mc.transform.lossyScale);

        else if (c.TryGetComponent(out SphereCollider sc))
            DrawSphereCollider(sc);

        else if (c.TryGetComponent(out BoxCollider bc))
            DrawBoxCollider(bc);
    }
}
