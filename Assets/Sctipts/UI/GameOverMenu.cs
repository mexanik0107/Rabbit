using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public CanvasGroup panelGroup;

    [Header("Тексты статистики")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text waveText;

    [Header("Кнопки")]
    public Button restartButton;
    public Button exitButton;

    private const string MENU_SCENE_NAME = "MenuScene";

    void Start()
    {
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);

        Hide();
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        // Позволяем выйти через Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }

    public void Show(int score, float timePlayed, int waves)
    {
        gameObject.SetActive(true);
        if (panelGroup != null)
        {
            panelGroup.alpha = 1;
            panelGroup.interactable = true;
            panelGroup.blocksRaycasts = true;
        }

        if (scoreText != null) scoreText.text = $"{score}";
        if (timeText != null) timeText.text = $"{FormatTime(timePlayed)}";
        if (waveText != null) waveText.text = $"{waves}";
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f; // Важно вернуть время обратно!
        string currentScene = SceneManager.GetActiveScene().name;

        // Используем наш загрузчик сцен, если он есть
        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene(currentScene);
        else SceneManager.LoadScene(currentScene);
    }

    private void ExitGame()
    {
        Time.timeScale = 1f;
        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene(MENU_SCENE_NAME);
        else SceneManager.LoadScene(MENU_SCENE_NAME);
    }

    // Форматирование секунд в ММ:СС
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds - minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}