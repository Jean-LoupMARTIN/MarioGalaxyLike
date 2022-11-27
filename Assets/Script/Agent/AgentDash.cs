using UnityEngine;


[RequireComponent(typeof(Agent))]
public class AgentDash : MonoBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] float shakeMagnitude = 10;
    [SerializeField] float shakeTime = 0.5f;
    [SerializeField] float chromaticAberration = 0.5f;
    [SerializeField] float lensDistortion = -0.5f;
    [SerializeField] AudioClip sound;

    Agent agent;

    void Awake()
    {
        agent = GetComponent<Agent>();
    }


    void OnEnable()
    {
        agent.Controller.A.OnPressDown.AddListener(Dash);
    }

    void OnDisable()
    {
        agent.Controller.A.OnPressDown.RemoveListener(Dash);
    }

    void Dash()
    {
        Vector3 dir;
        Vector3 stickL3 = agent.Controller.StickL3;

        if (stickL3 == Vector3.zero)
            dir = Vector3.forward;

        else if (agent.IsPlayer) {
            Quaternion camRotMatchUp = TransformExtension.RotationMatchUp(CameraExtension.current.transform.rotation, transform.up);
            dir = (transform.WorldToLocal(camRotMatchUp) * stickL3).normalized;
        }

        else dir = stickL3;


        agent.LocalVelocity = dir * speed;

        // play sound
        AudioSourceExtension.PlayClipAtPoint(sound, transform.position);

        if (agent.IsFocused)
        {
            // shake camera
            ShakeExtension.ShakeCamera(shakeMagnitude, shakeTime);

            // post processing
            PostProcessingExtension.StartChromaticAberrationCoroutine(chromaticAberration, 0, shakeTime);
            PostProcessingExtension.StartLensDistortionCoroutine(lensDistortion, 0, shakeTime);
        }
    }
}
