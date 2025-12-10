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

        // Гарантируем, что настройки скрыты при старте
        if (settingsMenu != null) settingsMenu.gameObject.SetActive(false);
    }

    private void InitializeButtons()
    {
        // 1. Resume Button
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
            // Авто-добавление звука клика
            if (resumeButton.GetComponent<UIButtonSound>() == null)
                resumeButton.gameObject.AddComponent<UIButtonSound>();
        }

        // 2. Settings Button
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
            if (settingsButton.GetComponent<UIButtonSound>() == null)
                settingsButton.gameObject.AddComponent<UIButtonSound>();
        }

        // 3. Main Menu Button
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            if (mainMenuButton.GetComponent<UIButtonSound>() == null)
                mainMenuButton.gameObject.AddComponent<UIButtonSound>();
        }
    }

    private void Update()
    {
        // Если Game Over, пауза не работает
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ПРИОРИТЕТ 1: Закрываем настройки, если они открыты
            if (settingsMenu != null && settingsMenu.gameObject.activeSelf)
            {
                settingsMenu.CloseSettings();
            }
            // ПРИОРИТЕТ 2: Переключаем паузу
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
        // Звук открытия паузы
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
        // Звук закрытия паузы (резюм)
        // Проверяем isPaused, чтобы звук не играл при старте игры (когда Hide вызывается в Start)
        if (isPaused && UIManager.Instance != null)
            UIManager.Instance.PlaySound(UIManager.Instance.menuCloseSound);

        Hide();
    }

    public void OpenSettings()
    {
        // --- ВОТ ЭТО БЫЛО ПРОПУЩЕНО ---
        // Играем звук открытия меню
        if (UIManager.Instance != null)
            UIManager.Instance.PlaySound(UIManager.Instance.menuOpenSound);
        // ------------------------------

        // Скрываем паузу визуально (но время стоит)
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
                // Callback: когда настройки закроются, снова показываем панель паузы
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
        ResumeGameTime();
        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene(MENU_SCENE_NAME);
        else SceneManager.LoadScene(MENU_SCENE_NAME);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void ResumeGameTime()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}