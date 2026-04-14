using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Daftar prefab hewan yang akan muncul (isi di Inspector)
    public GameObject[] animalPrefabs;

    // Batas-batas area spawn
    private float spawnRangeX = 20;
    private float spawnPosZ = 20;
    private float startDelay = 2;      // Menunggu 2 detik setelah game mulai
    private float spawnInterval = 1.5f; // Muncul hewan baru setiap 1.5 detik

    void Start()
    {
        // Untuk sekarang Start masih kosong, 
        // nanti di lesson selanjutnya kita isi buat timer otomatis.
        // Memanggil fungsi SpawnRandomAnimal secara otomatis
        InvokeRepeating("SpawnRandomAnimal", startDelay, spawnInterval);
    }

    void Update()
    {
        // Tekan tombol S untuk memunculkan hewan secara manual

    }

    // Fungsi khusus untuk mengacak jenis hewan dan lokasinya
    void SpawnRandomAnimal()
    {
        // 1. Mengacak index untuk memilih hewan dari array
        int animalIndex = Random.Range(0, animalPrefabs.Length);

        // 2. Mengacak posisi koordinat X (Kiri-Kanan)
        Vector3 spawnPos = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 0, spawnPosZ);

        // 3. Munculkan hewan hasil kocokan tadi di lokasi hasil kocokan tadi
        Instantiate(animalPrefabs[animalIndex], spawnPos, animalPrefabs[animalIndex].transform.rotation);
    }
}