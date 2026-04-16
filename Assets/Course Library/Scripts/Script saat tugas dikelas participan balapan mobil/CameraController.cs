using UnityEngine;
using System.Collections;

/// <summary>
/// CameraController — Kamera follow dengan efek cinematic saat intro dan finish.
/// Mode: Follow (normal) → Intro Zoom → Finish Orbit
/// </summary>
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Follow Settings")]
    public Transform target;
    public Vector3 followOffset  = new Vector3(0f, 4f, -8f);
    public float followSmoothing = 6f;

    [Header("Intro Zoom")]
    public Vector3 introOffset   = new Vector3(0f, 1.5f, -3f);   // dekat mobil
    public Vector3 normalOffset  = new Vector3(0f, 4f, -8f);      // posisi normal
    public float zoomDuration    = 2f;

    [Header("Finish Orbit")]
    public float orbitRadius     = 6f;
    public float orbitHeight     = 3f;
    public float orbitSpeed      = 60f;   // derajat/detik
    public float orbitDuration   = 5f;

    // ──────────────────────────────────────────────────────────────────────
    private enum CamMode { Follow, Zoom, Orbit }
    private CamMode mode = CamMode.Follow;

    private Vector3 currentOffset;
    private Transform orbitTarget;
    private float orbitAngle;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        currentOffset = followOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        switch (mode)
        {
            case CamMode.Follow:
                Vector3 desired = target.position + target.TransformDirection(currentOffset);
                transform.position = Vector3.Lerp(transform.position, desired, followSmoothing * Time.deltaTime);
                transform.LookAt(target.position + Vector3.up * 1f);
                break;

            case CamMode.Orbit:
                orbitAngle += orbitSpeed * Time.deltaTime;
                float rad = orbitAngle * Mathf.Deg2Rad;
                Vector3 orbitPos = orbitTarget.position
                    + new Vector3(Mathf.Sin(rad) * orbitRadius, orbitHeight, Mathf.Cos(rad) * orbitRadius);
                transform.position = orbitPos;
                transform.LookAt(orbitTarget.position + Vector3.up * 1f);
                break;
        }
    }

    // ─── Intro: zoom in ke mobil lalu mundur ke posisi normal ─────────────
    public void DoIntroZoom()
    {
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        mode = CamMode.Zoom;
        currentOffset = introOffset;
        float t = 0f;

        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            currentOffset = Vector3.Lerp(introOffset, normalOffset, t / zoomDuration);
            Vector3 pos = target.position + target.TransformDirection(currentOffset);
            transform.position = pos;
            transform.LookAt(target.position + Vector3.up * 1f);
            yield return null;
        }

        currentOffset = normalOffset;
        mode = CamMode.Follow;
    }

    // ─── Finish: orbit mengelilingi mobil secara cinematic ────────────────
    public void DoCinematicFinish(Transform car)
    {
        orbitTarget = car;
        orbitAngle  = 0f;
        mode        = CamMode.Orbit;
        StartCoroutine(StopOrbitAfter(orbitDuration));
    }

    IEnumerator StopOrbitAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        // Kembali ke follow setelah orbit selesai
        mode = CamMode.Follow;
    }
}
