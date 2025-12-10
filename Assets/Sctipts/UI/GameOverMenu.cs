using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup panelGroup;

    [Header("Stats Texts")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text waveText;

    [Header("Buttons")]
    public Button restartButton;
    public Button exitButton;

    private const string MENU_SCENE_NAME = "MenuScene";

    void Start()
    {
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);

        Hide();
    }

    // --- НОВОЕ: Слушаем кнопку ESC ---
    void Update()
    {
        // Если меню невидимо, ничего не делаем
        if (!gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }
    // ---------------------------------

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
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;

        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene(currentScene);
        else SceneManager.LoadScene(currentScene);
    }

    private void ExitGame()
    {
        Time.timeScale = 1f;
        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene(MENU_SCENE_NAME);
        else SceneManager.LoadScene(MENU_SCENE_NAME);
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds - minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}