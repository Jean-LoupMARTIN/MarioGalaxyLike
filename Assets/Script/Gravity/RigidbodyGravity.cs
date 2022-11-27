using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class RigidbodyGravity : MonoBehaviour
{
    [SerializeField] float gravityAceeleration = 40;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        rb.AddForce(gravityAceeleration * Gravity.Coef * Gravity.AtPos(transform.position).dir, ForceMode.Acceleration);
    }
}
