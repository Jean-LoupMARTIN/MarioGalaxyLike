using UnityEngine;



public static class TransformExtension
{

    public static Quaternion RotationMatchUp(Quaternion rotation, Vector3 up) => Quaternion.FromToRotation(rotation * Vector3.up, up) * rotation;
    public static void MatchUp(this ref Quaternion rotation, Vector3 up) => rotation = RotationMatchUp(rotation, up);
    public static void MatchUp(this Transform transform, Vector3 up) => transform.rotation = RotationMatchUp(transform.rotation, up);



    // https://forum.unity.com/threads/whats-the-math-behind-transform-transformpoint.107401/
    static public Vector3 WorldToLocal(Vector3 refPosition, Quaternion refRotation, Vector3 point)
    {
        return Quaternion.Inverse(refRotation) * (point - refPosition);
    }

    static public Vector3 LocalToWorld(Vector3 refPosition, Quaternion refRotation, Vector3 point)
    {
        return refRotation * point + refPosition;
    }


    static public Quaternion WorldToLocal(Quaternion refRotation, Quaternion rot)
    {
        return Quaternion.Inverse(refRotation) * rot;
    }

    static public Quaternion LocalToWorld(Quaternion refRotation, Quaternion rot)
    {
        return refRotation * rot;
    }

    static public Vector3 WorldToLocal(this Transform transform, Vector3 point) => WorldToLocal(transform.position, transform.rotation, point);
    static public Vector3 LocalToWorld(this Transform transform, Vector3 point) => LocalToWorld(transform.position, transform.rotation, point);

    static public Quaternion WorldToLocal(this Transform transform, Quaternion rot) => WorldToLocal(transform.rotation, rot);
    static public Quaternion LocalToWorld(this Transform transform, Quaternion rot) => LocalToWorld(transform.rotation, rot);
}
