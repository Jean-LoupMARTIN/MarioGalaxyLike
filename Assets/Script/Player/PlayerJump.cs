using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] float jumpVelocityY = 15;
    [SerializeField] float jumpVelocityXZ = 7.5f;
    [SerializeField] float cancelCheckGroundAfterJump = 0.25f;
    [SerializeField] AudioClip sound;
    float lastJumpTime = -1000;

    float TimeSinceLastJump => Time.time - lastJumpTime;

    Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnEnable()
    {
        player.Controller.X.OnPressDown.AddListener(Jump);
    }

    void OnDisable()
    {
        player.Controller.X.OnPressDown.RemoveListener(Jump);
    }

    void Jump()
    {
        if (player.Grounded) GroundJump();
        else AirJump();


        // play sound
        AudioSourceExtension.PlayClipAtPoint(sound, transform.position);

        // cancel CheckGround
        player.CancelCheckGroundTime = Time.time + cancelCheckGroundAfterJump;

        lastJumpTime = Time.time;
    }

    void GroundJump()
    {
        Vector3 localVelocity = player.LocalVelocity;
        localVelocity.y = jumpVelocityY;
        player.LocalVelocity = localVelocity;

        player.SetGrounded(false);
        player.SetGroundCollider(null);

    }

    void AirJump()
    {
        player.LocalVelocity = jumpVelocityY * Vector3.up + jumpVelocityXZ * player.Controller.StickL3;
    }
}
