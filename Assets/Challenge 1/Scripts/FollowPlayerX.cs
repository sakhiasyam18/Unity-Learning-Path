using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerX : MonoBehaviour
{
    public GameObject plane;
    // Kita beri nilai awal agar kamera berada di belakang (Z = -10) dan di atas (Y = 5) pesawat
    private Vector3 offset = new Vector3(0, 5, -10);

    void LateUpdate() // Ganti ke LateUpdate supaya halus
    {
        // Posisi kamera adalah posisi pesawat ditambah jarak (offset)
        transform.position = plane.transform.position + offset;
    }
}