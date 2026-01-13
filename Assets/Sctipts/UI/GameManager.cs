using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton: статический доступ к экземпляру класса откуда угодно (GameManager.Instance)
    public static GameManager Instance { get; private set; }

    [Header("Состояние игры")]
    public float totalSurvivalTime;
    public int currentScore;
    public int wavesSurvived;

    [Header("Настройки очков")]
    public int baseScorePerWave = 100;
    public float timeBonusCoefficient = 2000f;

    public bool IsGameActive => _isGameActive;

    private bool _isGameActive = true;
    private float _waveStartTime;
    private HUD _hud;

    void Awake()
    {
        // Реализация синглтона
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

        // Расчет бонуса за скорость прохождения
        float waveDuration = Time.time - _waveStartTime;
        waveDuration = Mathf.Max(waveDuration, 1f); // Защита от деления на ноль

        int wavePoints = waveIndex * baseScorePerWave;
        int timeBonus = Mathf.RoundToInt(timeBonusCoefficient / waveDuration);

        currentScore += wavePoints + timeBonus;
    }

    public void GameOver()
    {
        if (!_isGameActive) return;

        _isGameActive = false;

        // Показываем экран проигрыша
        GameOverMenu gameOverMenu = FindObjectOfType<GameOverMenu>(true); // true значит искать и среди выключенных объектов
        if (gameOverMenu != null)
        {
            gameOverMenu.Show(currentScore, totalSurvivalTime, wavesSurvived);
        }

        Time.timeScale = 0f; // Останавливаем время
    }
}