using UnityEngine;
using TMPro; // WAJIB TAMBAHKAN INI agar kodingan kenal TextMeshPro

public class DestroyOutOfBounds : MonoBehaviour
{
    private float topBound = 30;
    private float lowerBound = -10;

    public TextMeshProUGUI gameOverText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.z > topBound){
            Destroy(gameObject);
        }else if(transform.position.z < lowerBound){
            // Jika hewan lewat, nyalakan teksnya!
            if (gameOverText != null) {
                gameOverText.gameObject.SetActive(true);
            }
            Debug.Log("Game Selesai");
            Time.timeScale = 0;
            Destroy(gameObject);
        }
    }
}
