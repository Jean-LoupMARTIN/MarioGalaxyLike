using UnityEngine;



public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 10;
    [SerializeField] float rotSpeed = 10;

    Vector3 lastPos;
    Quaternion lastRot;

    void OnEnable()
    {
        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    void FixedUpdate()
    {
        if (target)
        {
            transform.position = lastPos;
            transform.rotation = lastRot;

            transform.position = Vector3   .Lerp(transform.position, target.position, Time.fixedDeltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.fixedDeltaTime * rotSpeed);

            lastPos = transform.position;
            lastRot = transform.rotation;
        }
    }
}
