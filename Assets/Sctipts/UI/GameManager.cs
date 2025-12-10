using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public float totalSurvivalTime;
    public int currentScore;
    public int wavesSurvived;

    [Header("Score Settings")]
    public int baseScorePerWave = 100;
    public float timeBonusCoefficient = 2000f;

    // --- НОВОЕ: Публичное свойство для чтения состояния ---
    public bool IsGameActive => _isGameActive;
    // -----------------------------------------------------

    private bool _isGameActive = true;
    private float _waveStartTime;
    private HUD _hud;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _hud = FindObjectOfType<HUD>();
    }

    void Start()
    {
        currentScore = 0;
        totalSurvivalTime = 0;
        wavesSurvived = 0;
        _waveStartTime = Time.time;
    }

    void Update()
    {
        if (_isGameActive)
        {
            totalSurvivalTime += Time.deltaTime;

            if (_hud != null)
            {
                _hud.UpdateGameTimer(totalSurvivalTime);
            }
        }
    }

    public void OnWaveStarted(int waveIndex)
    {
        _waveStartTime = Time.time;
    }

    public void OnWaveCompleted(int waveIndex)
    {
        wavesSurvived = waveIndex;
        float waveDuration = Time.time - _waveStartTime;
        waveDuration = Mathf.Max(waveDuration, 1f);

        int wavePoints = waveIndex * baseScorePerWave;
        int timeBonus = Mathf.RoundToInt(timeBonusCoefficient / waveDuration);
        currentScore += wavePoints + timeBonus;
    }

    public void GameOver()
    {
        if (!_isGameActive) return;

        _isGameActive = false;

        GameOverMenu gameOverMenu = FindObjectOfType<GameOverMenu>(true);
        if (gameOverMenu != null)
        {
            gameOverMenu.Show(currentScore, totalSurvivalTime, wavesSurvived);
        }

        Time.timeScale = 0f;
    }
}