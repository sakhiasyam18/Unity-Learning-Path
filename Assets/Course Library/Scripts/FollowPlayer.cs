using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new Vector3(0,5,-7);

    void start(){

    }
    // Menggunakan LateUpdate sesuai instruksi mentor agar kamera tidak bergetar (jitter)
    void LateUpdate()
    {
        // Menyusun posisi kamera berdasarkan posisi player ditambah jarak offset
        transform.position = player.transform.position + offset;
    }
}