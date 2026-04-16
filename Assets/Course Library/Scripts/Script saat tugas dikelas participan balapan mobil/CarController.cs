using UnityEngine;

/// <summary>
/// CarController — Kontrol mobil arcade-style dengan Rigidbody.
/// Mendukung: akselerasi, steer, rem, auto-brake saat finish.
/// Pasang pada GameObject mobil yang punya Rigidbody + Collider.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Gaya dorong ke depan (N)")]
    public float motorForce     = 1500f;
    [Tooltip("Gaya pengereman")]
    public float brakeForce     = 3000f;
    [Tooltip("Kecepatan maksimum (m/s)")]
    public float maxSpeed       = 25f;
    [Tooltip("Torsi belok")]
    public float steerTorque    = 200f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform[] groundCheckPoints;   // 4 titik di bawah mobil
    public float groundCheckDist = 0.3f;

    [Header("Audio")]
    public AudioSource engineAudio;
    public float minEnginePitch = 0.6f;
    public float maxEnginePitch = 2.0f;

    // ──────────────────────────────────────────────────────────────────────
    private Rigidbody rb;
    private bool controlEnabled;
    private bool autoBraking;
    private float inputAccel;
    private float inputSteer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);   // stabilitas
    }

    void Update()
    {
        if (!controlEnabled) return;

        inputAccel = Input.GetAxis("Vertical");    // W/S atau ↑↓
        inputSteer = Input.GetAxis("Horizontal");  // A/D atau ←→
    }

    void FixedUpdate()
    {
        if (autoBraking)
        {
            rb.AddForce(-rb.linearVelocity.normalized * brakeForce, ForceMode.Force);
            if (rb.linearVelocity.magnitude < 0.5f) rb.linearVelocity = Vector3.zero;
            return;
        }

        if (!controlEnabled) return;

        // ── Akselerasi ─────────────────────────────────────────────────────
        if (IsGrounded() && rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * inputAccel * motorForce, ForceMode.Force);
        }

        // ── Steer (hanya saat bergerak) ────────────────────────────────────
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            float steer = inputSteer * steerTorque * Mathf.Sign(inputAccel);
            rb.AddTorque(transform.up * steer, ForceMode.Force);
        }

        // ── Engine pitch ───────────────────────────────────────────────────
        if (engineAudio != null)
        {
            float speedRatio = rb.linearVelocity.magnitude / maxSpeed;
            engineAudio.pitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, speedRatio);
        }
    }

    // ─── Deteksi tanah di 4 titik ──────────────────────────────────────────
    bool IsGrounded()
    {
        foreach (var pt in groundCheckPoints)
            if (Physics.Raycast(pt.position, Vector3.down, groundCheckDist, groundLayer))
                return true;
        return false;
    }

    // ─── API Publik ────────────────────────────────────────────────────────
    public void EnableControl(bool enable)
    {
        controlEnabled = enable;
        if (!enable) { inputAccel = 0f; inputSteer = 0f; }
    }

    public void AutoBrake()
    {
        controlEnabled = false;
        autoBraking    = true;
    }
}
