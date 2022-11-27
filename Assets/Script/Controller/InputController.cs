using UnityEngine;


public class InputController : Controller
{
    static public InputController inst;
    
    InputActions inputActions;

    protected override void Awake()
    {
        base.Awake();

        inst = this;

        inputActions = new InputActions();

        inputActions.Map.StickL.performed += (ctx) => StickL = ctx.ReadValue<Vector2>();
        inputActions.Map.StickR.performed += (ctx) => StickR = ctx.ReadValue<Vector2>();

        inputActions.Map.StickL.canceled += (ctx) => StickL = Vector2.zero;
        inputActions.Map.StickR.canceled += (ctx) => StickR = Vector2.zero;

        inputActions.Map.A.started  += (ctx) => A.IsPressed = true;
        inputActions.Map.A.canceled += (ctx) => A.IsPressed = false;

        inputActions.Map.B.started  += (ctx) => B.IsPressed = true;
        inputActions.Map.B.canceled += (ctx) => B.IsPressed = false;

        inputActions.Map.X.started  += (ctx) => X.IsPressed = true;
        inputActions.Map.X.canceled += (ctx) => X.IsPressed = false;

        inputActions.Map.Y.started  += (ctx) => Y.IsPressed = true;
        inputActions.Map.Y.canceled += (ctx) => Y.IsPressed = false;
    }

    void OnEnable()
    {
        inputActions.Map.Enable();
    }

    void OnDisable()
    {
        inputActions.Map.Disable();
    }
}
