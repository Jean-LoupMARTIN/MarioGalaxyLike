using UnityEngine;



public class MoveWithRef : MonoBehaviour
{
    [SerializeField] Transform refTransform = null;
    Vector3 lastPos = Vector3.zero;
    Quaternion lastRot = Quaternion.identity;


    Rigidbody rb;


    void OnValidate()
    {
        if (Application.isPlaying)
            SetRefTransfrom(refTransform);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        SetRefTransfrom(refTransform);
    }

    void FixedUpdate()
    {
        if (refTransform && !refTransform.gameObject.isStatic)
        {
            Vector3    newPos = refTransform.position;
            Quaternion newRot = refTransform.rotation;

            if (newPos != lastPos || newRot != lastRot)
            {
                Vector3 localPos = TransformExtension.WorldToLocal(lastPos, lastRot, transform.position);
                transform.position = TransformExtension.LocalToWorld(newPos, newRot, localPos);


                Quaternion localRot = Quaternion.Inverse(lastRot) * transform.rotation;
                transform.rotation = newRot * localRot;

                if (rb) {
                    Vector3 localVelocity = Quaternion.Inverse(lastRot) * rb.velocity;
                    rb.velocity = newRot * localVelocity;
                }

                lastPos = newPos;
                lastRot = newRot;
            }
        }
    }

    
    public void SetRefTransfrom(Transform refTransform)
    {
        if (this.refTransform == refTransform)
            return;

        this.refTransform = refTransform;

        (lastPos, lastRot) = refTransform ? (refTransform.position, refTransform.rotation) :
                                            (Vector3.zero, Quaternion.identity);
    }
}
