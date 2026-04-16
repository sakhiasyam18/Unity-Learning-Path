using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float motorForce = 25000f; // Tenaga diperbesar untuk massa 800kg
    public float steerForce = 15f;    // Kecepatan putar sudut
    public float maxSpeed = 30f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundRayDist = 1.5f; // Lebih panjang agar pasti kena lantai

    [Header("Audio")]
    public AudioSource engineAudio;

    private Rigidbody rb;
    private bool controlEnabled;
    private float moveInput;
    private float turnInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Biar nggak gampang guling
    }

    void Update()
    {
        if (!controlEnabled) return;
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // Debugging visual: Garis merah di Scene view menunjukkan jangkauan sensor tanah
        Debug.DrawRay(transform.position, Vector3.down * groundRayDist, Color.red);
    }

    void FixedUpdate()
    {
        if (!controlEnabled) return;

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundRayDist, groundLayer);

        if (isGrounded)
        {
            // Maju Mundur (ForceMode.Acceleration agar tidak kaget)
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(transform.forward * moveInput * motorForce, ForceMode.Acceleration);
            }

            // Belok (Menggunakan Rotation agar lebih presisi)
            if (rb.linearVelocity.magnitude > 1f)
            {
                float turn = turnInput * steerForce * Time.fixedDeltaTime;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turn * 10, 0));
            }
        }

        // Audio Engine Pitch
        if (engineAudio != null)
        {
            engineAudio.pitch = Mathf.Lerp(0.6f, 2.0f, rb.linearVelocity.magnitude / maxSpeed);
        }
    }

    public void EnableControl(bool enable) => controlEnabled = enable;
    
    public void AutoBrake()
    {
        controlEnabled = false;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 2f);
    }
}