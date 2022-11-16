using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField] float speed = 10;


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
    }
}
