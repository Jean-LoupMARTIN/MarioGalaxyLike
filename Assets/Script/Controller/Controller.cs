using UnityEngine;
using UnityEngine.Events;

public abstract class Controller : MonoBehaviour
{
    Vector2 stickL;
    Vector2 stickR;
    [SerializeField] float stickMagnitudeMin = 0.1f;
    float stickSqrMagnitudeMin;

    public Vector2 StickL { get => stickL; set => stickL = value.sqrMagnitude >= stickSqrMagnitudeMin ? value : Vector2.zero; }
    public Vector2 StickR { get => stickR; set => stickR = value.sqrMagnitude >= stickSqrMagnitudeMin ? value : Vector2.zero; }

    public Vector3 StickL3 => new Vector3(StickL.x, 0, StickL.y);
    public Vector3 StickR3 => new Vector3(StickR.x, 0, StickR.y);

    public float StickArcValue(bool leftStick, bool xAxis, float arcAngle = 90)
    {
        Vector2 stick = leftStick ? stickL : stickR;
        float value;
        float angle = Vector2.Angle(xAxis ? Vector2.right : Vector2.up, stick);
        if      (angle < arcAngle)       value =  Mathf.InverseLerp(90, 0, angle);
        else if (angle > 180 - arcAngle) value = -Mathf.InverseLerp(180-arcAngle, 180, angle);
        else                             value =  0;
        value *= stick.magnitude;
        return value;
    }

    public Button A = new Button();
    public Button B = new Button();
    public Button X = new Button();
    public Button Y = new Button();

    public class Button
    {
        bool isPressed = false;
        public UnityEvent OnPressDown = new UnityEvent();
        public UnityEvent OnPressUp   = new UnityEvent();


        public bool IsPressed
        {
            get => isPressed;

            set {
                if (isPressed == value)
                    return;

                if (!isPressed && value)
                    OnPressDown.Invoke();

                if (isPressed && !value)
                    OnPressUp.Invoke();

                isPressed = value;
            }
        }
    }

    protected virtual void OnValidate()
    {
        Cache();
    }

    protected virtual void Awake()
    {
        Cache();
    }

    void Cache()
    {
        stickSqrMagnitudeMin = stickMagnitudeMin * stickMagnitudeMin;
    }
}
