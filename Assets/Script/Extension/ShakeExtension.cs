using EZCameraShake;
using UnityEngine;



public static class ShakeExtension
{
    public static void ShakeCamera(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
    {
        CameraShaker.Instance?.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
    }

    public static void ShakeCamera(float magnitude, float time)
    {
        CameraShaker.Instance?.ShakeOnce(magnitude, magnitude, 0, time);
    }
}
