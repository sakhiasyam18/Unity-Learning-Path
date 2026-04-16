using UnityEngine;

/// <summary>
/// Checkpoint — Sistem checkpoint opsional.
/// Mobil harus melewati semua checkpoint secara berurutan
/// sebelum garis finish dianggap sah.
/// Pasang pada Collider (isTrigger) di tiap checkpoint.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    [Header("Setup")]
    public int checkpointIndex;             // urutan, mulai dari 0
    public Renderer checkpointRenderer;     // visual checkpoint

    [Header("Colors")]
    public Color inactiveColor = new Color(1f, 1f, 0f, 0.3f);  // kuning transparan
    public Color activeColor   = new Color(0f, 1f, 0f, 0.5f);  // hijau saat dilewati

    public string carTag = "Player";

    void Start() => SetColor(inactiveColor);

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(carTag)) return;
        CheckpointManager.Instance?.OnCheckpointReached(checkpointIndex);
        SetColor(activeColor);
    }

    void SetColor(Color c)
    {
        if (checkpointRenderer != null)
            checkpointRenderer.material.color = c;
    }
}

// ─── Manager kecil untuk lacak urutan ─────────────────────────────────────────
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    public int totalCheckpoints;
    private int nextExpected = 0;
    public bool AllPassed => nextExpected >= totalCheckpoints;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnCheckpointReached(int index)
    {
        if (index == nextExpected)
        {
            nextExpected++;
            Debug.Log($"[Checkpoint] {index + 1}/{totalCheckpoints} ✓");
        }
    }
}
