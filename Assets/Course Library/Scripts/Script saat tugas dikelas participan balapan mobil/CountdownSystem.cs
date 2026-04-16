using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// CountdownSystem — Hitung mundur 3…2…1…GO!
/// Komunikasikan via callback Action setelah GO selesai.
/// </summary>
public class CountdownSystem : MonoBehaviour
{
    [Header("Timing")]
    public float stepDuration = 1f;   // durasi tiap angka (detik)
    public float goDuration   = 0.8f; // berapa lama "GO!" tampil

    [Header("Audio Clips")]
    public AudioClip beepClip;        // suara tiap angka
    public AudioClip goClip;          // suara GO!

    private AudioSource audioSrc;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        if (audioSrc == null) audioSrc = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Jalankan hitungan mundur lalu panggil onComplete saat selesai.
    /// </summary>
    public IEnumerator RunCountdown(Action onComplete)
    {
        int[] steps = { 3, 2, 1 };

        foreach (int step in steps)
        {
            UIManager.Instance.ShowCountdownText(step.ToString());
            PlayBeep();
            yield return new WaitForSeconds(stepDuration);
        }

        UIManager.Instance.ShowCountdownText("GO!");
        PlayGoSound();
        yield return new WaitForSeconds(goDuration);

        UIManager.Instance.HideCountdownText();
        onComplete?.Invoke();
    }

    void PlayBeep()
    {
        if (beepClip != null) audioSrc.PlayOneShot(beepClip);
    }

    void PlayGoSound()
    {
        if (goClip != null) audioSrc.PlayOneShot(goClip);
    }
}
