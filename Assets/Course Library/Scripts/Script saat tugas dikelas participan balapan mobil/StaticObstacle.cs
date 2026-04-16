using UnityEngine;

/// <summary>
/// StaticObstacle — Rintangan diam (batu, tembok, drum, dll).
/// Saat ditabrak mobil:
///   • Bisa opsional: bergetar / jatuh / hancur
///   • Play crash SFX
///   • Spawn partikel debris
/// Pasang pada GameObject rintangan statis.
/// Rigidbody opsional (jika ingin bisa ditabrak).
/// </summary>
public class StaticObstacle : MonoBehaviour
{
    public enum ReactionType { None, Shake, Topple, Shatter }

    [Header("Behavior")]
    public ReactionType reaction    = ReactionType.Topple;
    public float        shakeDuration = 0.3f;
    public float        shakeMagnitude = 0.05f;

    [Header("VFX")]
    public ParticleSystem debrisParticle;
    public GameObject     shatterPrefab;     // opsional: prefab hancur

    [Header("Tags")]
    public string carTag = "Player";

    // ──────────────────────────────────────────────────────────────────────
    private bool triggered;
    private Vector3 originPos;

    void Start() => originPos = transform.position;

    void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.CompareTag(carTag)) return;
        if (triggered) return;
        triggered = true;

        AudioManager.Instance?.PlayCrash();
        debrisParticle?.Play();

        switch (reaction)
        {
            case ReactionType.Shake:   StartCoroutine(ShakeRoutine()); break;
            case ReactionType.Topple:  Topple(col); break;
            case ReactionType.Shatter: Shatter(); break;
        }
    }

    // ─── Getaran ──────────────────────────────────────────────────────────
    System.Collections.IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float z = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.position = originPos + new Vector3(x, 0f, z);
            yield return null;
        }
        transform.position = originPos;
        triggered = false; // bisa ditabrak lagi
    }

    // ─── Jatuh ────────────────────────────────────────────────────────────
    void Topple(Collision col)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 30f;
        }
        rb.isKinematic = false;
        Vector3 dir = col.contacts[0].point - transform.position;
        dir.y = 0.5f;
        rb.AddForce(-dir.normalized * 300f + Vector3.up * 200f, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 100f, ForceMode.Impulse);
    }

    // ─── Hancur ───────────────────────────────────────────────────────────
    void Shatter()
    {
        if (shatterPrefab != null)
            Instantiate(shatterPrefab, transform.position, transform.rotation);
        Destroy(gameObject, 0.05f);
    }
}
