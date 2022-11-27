using UnityEngine;



public class CameraExtension : MonoBehaviour
{
    static public Camera current;

    void Awake()
    {
        current = Camera.main;
    }
}
