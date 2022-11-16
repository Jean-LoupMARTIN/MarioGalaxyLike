using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class PostProcessingExtension : MonoBehaviour
{
    static PostProcessingExtension inst;

    Volume volume;
    ChromaticAberration chromaticAberration;
    LensDistortion lensDistortion;

    void Awake()
    {
        inst = this;
        Cache();
    }


    void Cache()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out lensDistortion);
    }

    public static void StartChromaticAberrationCoroutine(float addIntensity, float fadeInTime, float fadeOutTime)
    {
        if (inst && inst.chromaticAberration)
            inst.StartCoroutine(inst.PostProCoroutine(addIntensity, fadeInTime, fadeOutTime, x => inst.chromaticAberration.intensity.value += x));
    }

    public static void StartLensDistortionCoroutine(float addIntensity, float fadeInTime, float fadeOutTime)
    {
        if (inst && inst.lensDistortion)
            inst.StartCoroutine(inst.PostProCoroutine(addIntensity, fadeInTime, fadeOutTime, x => inst.lensDistortion.intensity.value += x));
    }

    IEnumerator PostProCoroutine(float intensity, float fadeInTime, float fadeOutTime, Action<float> AddItensity)
    {
        float t = 0;
        float progress;

        float lastIntensity = 0;
        float nextIntensity;
        float dIntensity;

        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            progress = Mathf.Clamp01(t / fadeInTime);

            nextIntensity = intensity * progress;
            dIntensity = nextIntensity - lastIntensity;
            lastIntensity = nextIntensity;

            AddItensity(dIntensity);

            yield return new WaitForEndOfFrame();
        }

        t = 0;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            progress = Mathf.Clamp01(t / fadeOutTime);

            nextIntensity = intensity * (1 - progress);
            dIntensity = nextIntensity - lastIntensity;
            lastIntensity = nextIntensity;

            chromaticAberration.intensity.value += dIntensity;
            AddItensity(dIntensity);

            yield return new WaitForEndOfFrame();
        }
    }
}
