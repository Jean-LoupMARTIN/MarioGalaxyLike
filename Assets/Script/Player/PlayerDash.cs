using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] float shakeMagnitude = 10;
    [SerializeField] float shakeTime = 0.5f;
    [SerializeField] float chromaticAberration = 0.5f;
    [SerializeField] float lensDistortion = -0.5f;
    [SerializeField] AudioClip sound;

    Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }


    void OnEnable()
    {
        player.Controller.A.OnPressDown.AddListener(Dash);
    }

    void OnDisable()
    {
        player.Controller.A.OnPressDown.RemoveListener(Dash);
    }

    void Dash()
    {
        Vector3 stickL3 = player.Controller.StickL3;
        Vector3 dir = stickL3 != Vector3.zero ? stickL3 : Vector3.forward;
        dir.Normalize();
        player.LocalVelocity = dir * speed;

        // play sound
        AudioSourceExtension.PlayClipAtPoint(sound, transform.position);

        // shake camera
        ShakeExtension.ShakeCamera(shakeMagnitude, shakeTime);

        // post processing
        PostProcessingExtension.StartChromaticAberrationCoroutine(chromaticAberration, 0, shakeTime);
        PostProcessingExtension.StartLensDistortionCoroutine(lensDistortion, 0, shakeTime); 
    }
}
