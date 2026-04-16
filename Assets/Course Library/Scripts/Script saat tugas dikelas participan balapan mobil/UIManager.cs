using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// UIManager — Mengatur semua panel UI:
///   • StartPanel     : tombol START + info level
///   • CountdownPanel : angka 3,2,1,GO!
///   • HUD            : timer real-time saat bermain
///   • WinPanel       : waktu finish + tombol next/menu
///   • LosePanel      : tombol retry/menu
///
/// Gunakan TextMeshPro (TMP) untuk teks.
/// Pasang semua referensi via Inspector.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // ─── Start Panel ────────────────────────────────────────────────────────
    [Header("Start Panel")]
    public GameObject startPanel;
    public TextMeshProUGUI levelTitleText;
    public TextMeshProUGUI levelDescText;
    public Button startButton;

    // ─── Countdown Panel ────────────────────────────────────────────────────
    [Header("Countdown Panel")]
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;
    public Animator countdownAnimator;       // opsional: animasi scale bounce
    private static readonly int CountHash = Animator.StringToHash("Pop");

    // ─── HUD ────────────────────────────────────────────────────────────────
    [Header("HUD")]
    public GameObject hudPanel;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI speedText;        // opsional: tampilkan kecepatan
    private CarController car;

    // ─── Win Panel ──────────────────────────────────────────────────────────
    [Header("Win Panel")]
    public GameObject winPanel;
    public TextMeshProUGUI winTimeText;
    public Button nextLevelButton;
    public Button winMenuButton;

    // ─── Lose Panel ─────────────────────────────────────────────────────────
    [Header("Lose Panel")]
    public GameObject losePanel;
    public Button retryButton;
    public Button loseMenuButton;

    // ────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Sembunyikan semua panel di awal
        HideAll();
    }

    void Start()
    {
        // Pasang event button
        startButton.onClick.AddListener(() => GameManager.Instance.OnStartButtonPressed());
        nextLevelButton.onClick.AddListener(() => GameManager.Instance.LoadNextLevel());
        winMenuButton.onClick.AddListener(() => GameManager.Instance.LoadMainMenu());
        retryButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());
        loseMenuButton.onClick.AddListener(() => GameManager.Instance.LoadMainMenu());

        car = GameManager.Instance.carController;
    }

    void Update()
    {
        // Update HUD timer & speed real-time
        if (hudPanel.activeSelf && GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            float t = GameManager.Instance.ElapsedTime;
            timerText.text = FormatTime(t);

            if (speedText != null && car != null)
            {
                float kmh = car.GetComponent<Rigidbody>().linearVelocity.magnitude * 3.6f;
                speedText.text = $"{kmh:F0} km/h";
            }
        }
    }

    // ─── Tampilkan / Sembunyikan ─────────────────────────────────────────────
    public void ShowStartPanel(int level, string desc)
    {
        startPanel.SetActive(true);
        levelTitleText.text = $"LEVEL {level}";
        levelDescText.text  = desc;
    }

    public void HideStartPanel() => startPanel.SetActive(false);

    public void ShowCountdownText(string txt)
    {
        countdownPanel.SetActive(true);
        countdownText.text = txt;
        countdownAnimator?.SetTrigger(CountHash);
    }

    public void HideCountdownText() => countdownPanel.SetActive(false);

    public void ShowHUD()
    {
        hudPanel.SetActive(true);
        timerText.text = "00:00.000";
    }

    public void ShowWinPanel(float time)
    {
        winPanel.SetActive(true);
        winTimeText.text = $"Waktu: {FormatTime(time)}";
        StartCoroutine(AnimatePanel(winPanel));
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        StartCoroutine(AnimatePanel(losePanel));
    }

    // ─── Helper ─────────────────────────────────────────────────────────────
    void HideAll()
    {
        startPanel.SetActive(false);
        countdownPanel.SetActive(false);
        hudPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    static string FormatTime(float seconds)
    {
        int min = (int)(seconds / 60);
        int sec = (int)(seconds % 60);
        int ms  = (int)((seconds - Mathf.Floor(seconds)) * 1000);
        return $"{min:00}:{sec:00}.{ms:000}";
    }

    IEnumerator AnimatePanel(GameObject panel)
    {
        // Scale bounce in
        panel.transform.localScale = Vector3.zero;
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            float scale = Mathf.SmoothStep(0f, 1f, t / 0.4f);
            panel.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        panel.transform.localScale = Vector3.one;
    }
}
