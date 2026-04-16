using UnityEngine;
using System.Collections;

/// <summary>
/// Ramp — Pasang pada GameObject ramp.
/// Memberi dorongan ke atas saat mobil menabrak trigger ramp.
/// Juga deteksi landing setelah lompatan.
/// </summary>
public class Ramp : MonoBehaviour
{
    [Header("Launch Settings")]
    [Tooltip("Gaya dorongan ke atas saat melewati ramp")]
    public float launchForce       = 800f;
    [Tooltip("Sudut dorongan dari depan (derajat). 0 = lurus ke atas, positif = maju lebih jauh")]
    [Range(0f, 60f)]
    public float launchAngleDeg    = 30f;
    [Tooltip("Minimal kecepatan mobil agar ramp aktif (m/s)")]
    public float minSpeedToLaunch  = 3f;

    [Header("Landing Detect")]
    public LayerMask groundLayer;
    public float landCheckDist     = 1.2f;
    public float airTimeThreshold  = 0.5f; // detik di udara sebelum dihitung "flying"

    [Header("Visual FX")]
    public ParticleSystem dustParticle;      // debu saat naik ramp
    public TrailRenderer   jumpTrail;        // trail di udara

    [Header("Tags")]
    public string carTag = "Player";

    // ──────────────────────────────────────────────────────────────────────
    private Rigidbody carRB;
    private bool inAir;
    private float airTimer;

    // ─── Trigger ramp (gunakan Box/MeshCollider isTrigger pada area atas ramp)
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(carTag)) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;
        if (rb.linearVelocity.magnitude < minSpeedToLaunch) return;

        carRB = rb;

        // Hitung arah peluncuran: forward ramp + up
        Vector3 launchDir = Quaternion.AngleAxis(-launchAngleDeg, transform.right) * transform.up;
        rb.AddForce(launchDir * launchForce, ForceMode.Impulse);

        AudioManager.Instance?.PlayJump();
        dustParticle?.Play();

        if (jumpTrail != null) jumpTrail.emitting = true;

        inAir     = true;
        airTimer  = 0f;
        StartCoroutine(LandingWatcher(rb));
    }

    // ─── Pantau kapan mobil landing ────────────────────────────────────────
    IEnumerator LandingWatcher(Rigidbody rb)
    {
        // Tunggu sebentar agar raycast tidak langsung mendeteksi ramp itu sendiri
        yield return new WaitForSeconds(0.3f);

        while (true)
        {
            if (rb == null) yield break;

            airTimer += Time.deltaTime;

            bool grounded = Physics.Raycast(rb.transform.position, Vector3.down, landCheckDist, groundLayer);
            if (grounded && airTimer > airTimeThreshold)
            {
                OnLanded();
                yield break;
            }
            yield return null;
        }
    }

    void OnLanded()
    {
        inAir = false;
        AudioManager.Instance?.PlayLand();
        dustParticle?.Play();
        if (jumpTrail != null) jumpTrail.emitting = false;
    }
}
