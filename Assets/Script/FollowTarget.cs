using UnityEngine;



public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 10;
    [SerializeField] float rotSpeed = 10;

    void FixedUpdate()
    {
        if (target)
        {
            transform.position = Vector3   .Lerp(transform.position, target.position, Time.fixedDeltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.fixedDeltaTime * rotSpeed);
        }
    }
}
