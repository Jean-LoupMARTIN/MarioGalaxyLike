using UnityEngine;



public class Rotate : MonoBehaviour
{
    [SerializeField] Vector3 rotation = new Vector3(0, 90, 0);


    void FixedUpdate()
    {
        transform.Rotate(rotation * Time.fixedDeltaTime);
    }
}
