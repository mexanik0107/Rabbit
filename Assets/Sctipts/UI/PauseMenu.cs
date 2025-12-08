using UnityEngine;
using UnityEngine.UI;

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
    
    private void Start()
    {
        InitializeButtons();
        Hide();
    }
    
    private void InitializeButtons()
    {
        // Кнопка "Продолжить"
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(() =>
            {
                PlaySound();
                ResumeGame();
            });
        }
        
        // Кнопка "Настройки"
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() =>
            {
                PlaySound();
                ShowSettings();
            });
        }
        
        // Кнопка "Выход в главное меню"
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(() =>
            {
                PlaySound();
                ReturnToMainMenu();
            });
        }
    }
    
    private void Update()
    {
        // Обработка ESC только в игровой сцене
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            Show();
        }
    }
    
    public void Show()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCanvasGroup(pauseMenuPanel);
            UIManager.Instance.PlaySound(UIManager.Instance.menuOpenSound);
        }
        else if (pauseMenuPanel != null)
        {
            pauseMenuPanel.alpha = 1;
            pauseMenuPanel.interactable = true;
            pauseMenuPanel.blocksRaycasts = true;
        }
        
        PauseGame();
    }
    
    public void Hide()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideCanvasGroup(pauseMenuPanel);
            UIManager.Instance.PlaySound(UIManager.Instance.menuCloseSound);
        }
        else if (pauseMenuPanel != null)
        {
            pauseMenuPanel.alpha = 0;
            pauseMenuPanel.interactable = false;
            pauseMenuPanel.blocksRaycasts = false;
        }
        
        ResumeGameTime();
    }
    
    public void ResumeGame()
    {
        Hide();
    }
    
    public void ShowSettings()
    {
        Hide();
        if (settingsMenu != null)
        {
            settingsMenu.Show();
        }
    }
    
    private void ReturnToMainMenu()
    {
        // Сбрасываем паузу
        ResumeGameTime();
        
        // Загружаем меню
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("MenuScene");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        }
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
    
    private void PlaySound()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.PlaySound(UIManager.Instance.buttonClickSound);
        }
    }
}