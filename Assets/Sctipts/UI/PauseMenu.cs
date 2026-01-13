using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public CanvasGroup pauseMenuPanel;
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;

    [Header("Настройки")]
    public SettingsMenu settingsMenu;

    private bool isPaused = false;
    private const string MENU_SCENE_NAME = "MenuScene";

    private void Start()
    {
        InitializeButtons();
        Hide();

        if (settingsMenu != null) settingsMenu.gameObject.SetActive(false);
    }

    private void InitializeButtons()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
            if (resumeButton.GetComponent<UIButtonSound>() == null)
                resumeButton.gameObject.AddComponent<UIButtonSound>();
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
            if (settingsButton.GetComponent<UIButtonSound>() == null)
                settingsButton.gameObject.AddComponent<UIButtonSound>();
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            if (mainMenuButton.GetComponent<UIButtonSound>() == null)
                mainMenuButton.gameObject.AddComponent<UIButtonSound>();
        }
    }

    private void Update()
    {
        // Если игра закончилась, пауза недоступна
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Приоритет 1: Закрыть настройки, если они открыты
            if (settingsMenu != null && settingsMenu.gameObject.activeSelf)
            {
                settingsMenu.CloseSettings();
            }
            // Приоритет 2: Переключить паузу
            else
            {
                TogglePause();
            }
        }
    }

    private void TogglePause()
    {
        if (isPaused) ResumeGame();
        else Show();
    }

    public void Show()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.PlaySound(UIManager.Instance.menuOpenSound);

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.alpha = 1;
            pauseMenuPanel.interactable = true;
            pauseMenuPanel.blocksRaycasts = true;
        }
        PauseGame();
    }

    public void Hide()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.alpha = 0;
            pauseMenuPanel.interactable = false;
            pauseMenuPanel.blocksRaycasts = false;
        }
        ResumeGameTime();
    }

    public void ResumeGame()
    {
        if (isPaused && UIManager.Instance != null)
            UIManager.Instance.PlaySound(UIManager.Instance.menuCloseSound);

        Hide();
    }

    public void OpenSettings()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.PlaySound(UIManager.Instance.menuOpenSound);

        // Прячем меню паузы, но игру оставляем на паузе
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.alpha = 0;
            pauseMenuPanel.interactable = false;
            pauseMenuPanel.blocksRaycasts = false;
        }

        if (settingsMenu != null)
        {
            settingsMenu.Open(() =>
            {
                // Callback: возвращаем меню паузы, когда закрыли настройки
                if (pauseMenuPanel != null)
                {
                    pauseMenuPanel.alpha = 1;
                    pauseMenuPanel.interactable = true;
                    pauseMenuPanel.blocksRaycasts = true;
                }
            });
        }
    }

    private void ReturnToMainMenu()
    {
        ResumeGameTime(); // Обязательно восстанавливаем время перед сменой сцены
        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene(MENU_SCENE_NAME);
        else SceneManager.LoadScene(MENU_SCENE_NAME);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // Заморозка времени
        isPaused = true;
    }

    private void ResumeGameTime()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}