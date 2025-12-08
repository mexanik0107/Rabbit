using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public CanvasGroup mainMenuPanel;
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;
    
    [Header("Настройки")]
    public SettingsMenu settingsMenu;
    
    private void Start()
    {
        InitializeButtons();
        Show();
    }
    
    private void InitializeButtons()
    {
        // Кнопка "Играть"
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() =>
            {
                PlaySound();
                PlayGame();
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
        
        // Кнопка "Выход"
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() =>
            {
                PlaySound();
                ExitGame();
            });
        }
    }
    
    public void PlayGame()
    {
        // Используем SceneLoader для плавной загрузки
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("GameScene");
        }
        else
        {
            // Если SceneLoader не используется
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
    }
    
    public void ShowSettings()
    {
        Hide();
        if (settingsMenu != null)
        {
            settingsMenu.Show();
        }
    }
    
    public void Show()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCanvasGroup(mainMenuPanel);
        }
        else if (mainMenuPanel != null)
        {
            mainMenuPanel.alpha = 1;
            mainMenuPanel.interactable = true;
            mainMenuPanel.blocksRaycasts = true;
        }
    }
    
    public void Hide()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideCanvasGroup(mainMenuPanel);
        }
        else if (mainMenuPanel != null)
        {
            mainMenuPanel.alpha = 0;
            mainMenuPanel.interactable = false;
            mainMenuPanel.blocksRaycasts = false;
        }
    }
    
    private void PlaySound()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.PlaySound(UIManager.Instance.buttonClickSound);
        }
    }
    
    private void ExitGame()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.QuitGame();
        }
        else
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}