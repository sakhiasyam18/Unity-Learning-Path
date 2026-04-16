using UnityEngine;
using System.Collections;

/// <summary>
/// FinishLine — Pasang pada GameObject garis finish (harus ada Collider dengan isTrigger = true).
/// Efek: glow pulse, particle confetti, bendera animasi.
/// </summary>
public class FinishLine : MonoBehaviour
{
    [Header("Glow Effect")]
    public Renderer[] glowRenderers;        // MeshRenderer garis / gerbang
    public Color idleColor   = Color.white;
    public Color activeColor = Color.yellow;
    public float glowPulseSpeed = 2f;

    [Header("Particles")]
    public ParticleSystem confettiParticle; // buat di Unity: Particle System
    public ParticleSystem glowBurstParticle;

    [Header("Flag Animation")]
    public Animator flagAnimator;           // animator bendera kotak-kotak
    private static readonly int WaveHash = Animator.StringToHash("Wave");

    [Header("Tag")]
    public string carTag = "Player";

    // ─── Glow idle pulse ───────────────────────────────────────────────────
    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState == GameManager.GameState.Finished) return;

        float t = Mathf.PingPong(Time.time * glowPulseSpeed, 1f);
        Color col = Color.Lerp(idleColor, activeColor, t);
        foreach (var r in glowRenderers)
            r.material.SetColor("_EmissionColor", col);
    }

    // ─── Trigger ───────────────────────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(carTag)) return;
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        TriggerFinishEffects();
        GameManager.Instance.OnFinishReached(true);
    }

    void TriggerFinishEffects()
    {
        // Aktifkan partikel
        confettiParticle?.Play();
        glowBurstParticle?.Play();

        // Bendera wave
        flagAnimator?.SetTrigger(WaveHash);

        // Set glow ke active penuh
        foreach (var r in glowRenderers)
            r.material.SetColor("_EmissionColor", activeColor * 3f);
    }
}
