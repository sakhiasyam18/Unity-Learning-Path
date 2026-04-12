using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    public float speed = 15.0f; // Beri nilai default agar tidak diam di tempat
    public float rotationSpeed = 100.0f;
    public float verticalInput;
    public float horizontalInput;

    void Update() // Mentor biasanya menyarankan Update untuk input yang responsif
    {
        // 1. Ambil input dari keyboard (Panah Atas/Bawah atau W/S)
        verticalInput = Input.GetAxis("Vertical");

        // 2. Gerakkan pesawat ke depan secara konstan
        // Kamu perlu menambahkan Time.deltaTime agar kecepatannya normal (meter per detik)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 3. Putar pesawat ke atas/bawah berdasarkan input pemain
        // Kamu harus mengalikan dengan verticalInput agar pesawat hanya berputar saat tombol ditekan
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime * verticalInput);

        // Ambil input kiri/kanan (tombol A/D atau Panah Kiri/Kanan)
        horizontalInput = Input.GetAxis("Horizontal");

        // Putar pesawat ke samping
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * horizontalInput);

    }
}