using UnityEngine;

public class SpinPropellerX : MonoBehaviour
{
    private float propSpeed = 1000.0f;

    void Update()
    {
        // Memutar baling-baling pada sumbu Z (Forward)
        transform.Rotate(Vector3.forward * propSpeed * Time.deltaTime);
    }
}