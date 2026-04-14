using UnityEngine;

public class DetectCollisions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ini fungsi untuk mendeteksi tabrakan trigger nya 
    private void OnTriggerEnter(Collider other){
        // hancurkan peluru nya kalau sudah nabrak
        Destroy(gameObject);

        //hancurkan benda yang ditabrak 
        Destroy(other.gameObject);
    }
}
