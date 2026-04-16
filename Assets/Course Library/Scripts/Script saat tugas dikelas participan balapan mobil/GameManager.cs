using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager — Singleton pusat kendali semua state game.
/// State: IDLE → COUNTDOWN → PLAYING → FINISHED
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Idle, Countdown, Playing, Finished }
    public GameState CurrentState { get; private set; } = GameState.Idle;

    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelDescription = "Jalan Licin & Rintangan Bergerak";

    [Header("References")]
    public CarController carController;
    public CameraController cameraController;
    public UIManager uiManager;
    public AudioManager audioManager;
    public CountdownSystem countdownSystem;

    // Timer
    private float elapsedTime;
    public float ElapsedTime => elapsedTime;
    private bool timerRunning;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Set state awal: mobil diam, tampilkan UI start
        SetState(GameState.Idle);
        uiManager.ShowStartPanel(levelNumber, levelDescription);
        cameraController.DoIntroZoom();
        audioManager.PlayEngineIdle();
    }

    void Update()
    {
        if (timerRunning)
            elapsedTime += Time.deltaTime;
    }

    // ─── Dipanggil tombol START di UI ───────────────────────────────────────
    public void OnStartButtonPressed()
    {
        if (CurrentState != GameState.Idle) return;
        uiManager.HideStartPanel();
        SetState(GameState.Countdown);
        StartCoroutine(countdownSystem.RunCountdown(OnCountdownFinished));
    }

    // ─── Setelah hitung mundur selesai ──────────────────────────────────────
    void OnCountdownFinished()
    {
        SetState(GameState.Playing);
        carController.EnableControl(true);
        audioManager.PlayEngineRevving();
        audioManager.PlayBGMusic();
        timerRunning = true;
        uiManager.ShowHUD();
    }

    // ─── Dipanggil FinishLine saat mobil menyentuh garis ────────────────────
    public void OnFinishReached(bool success)
    {
        if (CurrentState != GameState.Playing) return;
        SetState(GameState.Finished);

        timerRunning = false;
        carController.EnableControl(false);
        carController.AutoBrake();

        audioManager.StopBGMusic();

        if (success)
        {
            audioManager.PlayWinSFX();
            cameraController.DoCinematicFinish(carController.transform);
            uiManager.ShowWinPanel(elapsedTime);
        }
        else
        {
            audioManager.PlayLoseSFX();
            uiManager.ShowLosePanel();
        }
    }

    // ─── Restart / Next level ────────────────────────────────────────────────
    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void LoadNextLevel()  => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    public void LoadMainMenu()   => SceneManager.LoadScene(0);

    void SetState(GameState s) => CurrentState = s;
}
