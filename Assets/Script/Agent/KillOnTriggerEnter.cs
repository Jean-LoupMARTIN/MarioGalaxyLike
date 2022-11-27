using UnityEngine;


public class KillOnTriggerEnter : MonoBehaviour
{
    [SerializeField] LayerMask layer;
    [SerializeField] GameObject destroyOnKill;

    void OnTriggerEnter(Collider other)
    {
        if (layer == (layer | (1 << other.gameObject.layer)))
            Kill();
    }

    void Kill()
    {
        if (destroyOnKill)
            Destroy(destroyOnKill);
    }
}

