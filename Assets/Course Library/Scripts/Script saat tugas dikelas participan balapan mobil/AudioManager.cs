using UnityEngine;

/// <summary>
/// AudioManager — Pusat pengaturan semua suara:
///   • BGM (background music)
///   • Engine idle & revving
///   • Win / Lose SFX
///   • Collision SFX
/// Pasang di GameObject AudioManager di scene.
/// Isi AudioSource dan AudioClip via Inspector.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource bgmSource;       // looping, volume rendah
    public AudioSource engineSource;    // looping, pitch berubah
    public AudioSource sfxSource;       // one-shot

    [Header("BGM Clips")]
    public AudioClip bgmClip;

    [Header("Engine Clips")]
    public AudioClip engineIdleClip;
    public AudioClip engineRevClip;

    [Header("SFX Clips")]
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip crashClip;
    public AudioClip jumpClip;
    public AudioClip landClip;

    [Header("Volume")]
    [Range(0f, 1f)] public float bgmVolume    = 0.4f;
    [Range(0f, 1f)] public float engineVolume = 0.6f;
    [Range(0f, 1f)] public float sfxVolume    = 0.9f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        bgmSource.loop    = true;
        bgmSource.volume  = bgmVolume;

        engineSource.loop   = true;
        engineSource.volume = engineVolume;
    }

    // ─── BGM ────────────────────────────────────────────────────────────────
    public void PlayBGMusic()
    {
        if (bgmClip == null) return;
        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }

    public void StopBGMusic()
    {
        bgmSource.Stop();
    }

    // ─── Engine ─────────────────────────────────────────────────────────────
    public void PlayEngineIdle()
    {
        if (engineIdleClip == null) return;
        engineSource.clip  = engineIdleClip;
        engineSource.pitch = 1f;
        engineSource.Play();
    }

    public void PlayEngineRevving()
    {
        if (engineRevClip == null) return;
        engineSource.clip  = engineRevClip;
        engineSource.pitch = 1f;
        engineSource.Play();
    }

    public void StopEngine() => engineSource.Stop();

    // ─── SFX One-shot ───────────────────────────────────────────────────────
    public void PlayWinSFX()   => Play(winClip);
    public void PlayLoseSFX()  => Play(loseClip);
    public void PlayCrash()    => Play(crashClip);
    public void PlayJump()     => Play(jumpClip);
    public void PlayLand()     => Play(landClip);

    void Play(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
