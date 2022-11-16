using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class RigidbodyGravity : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        rb.AddForce(Gravity.AcceAtPos(transform.position), ForceMode.Acceleration);
    }
}
