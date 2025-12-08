using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Компоненты")]
    public CanvasGroup settingsPanel;
    public Slider volumeSlider;
    public Button backButton;
    public Toggle fullscreenToggle;
    
    private void Start()
    {
        InitializeComponents();
        LoadSettings();
        Hide(); // Скрываем при старте
    }
    
    private void InitializeComponents()
    {
        // Слайдер громкости
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        
        // Кнопка назад
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() =>
            {
                PlaySound();
                Hide();
            });
        }
        
        // Переключатель полноэкранного режима
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }
    }
    
    public void Show()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCanvasGroup(settingsPanel);
            UIManager.Instance.PlaySound(UIManager.Instance.menuOpenSound);
        }
        else if (settingsPanel != null)
        {
            settingsPanel.alpha = 1;
            settingsPanel.interactable = true;
            settingsPanel.blocksRaycasts = true;
        }
        
        LoadSettings();
    }
    
    public void Hide()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideCanvasGroup(settingsPanel);
            UIManager.Instance.PlaySound(UIManager.Instance.menuCloseSound);
        }
        else if (settingsPanel != null)
        {
            settingsPanel.alpha = 0;
            settingsPanel.interactable = false;
            settingsPanel.blocksRaycasts = false;
        }
        
        SaveSettings();
        
        // Возвращаемся к предыдущему меню
        ReturnToPreviousMenu();
    }
    
    private void LoadSettings()
    {
        // Громкость
        if (volumeSlider != null)
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = volume;
            ApplyVolume(volume);
        }
        
        // Полноэкранный режим
        if (fullscreenToggle != null)
        {
            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            fullscreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }
    }
    
    private void SaveSettings()
    {
        // Сохраняем громкость
        if (volumeSlider != null)
        {
            PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
        }
        
        // Сохраняем настройки полноэкранного режима
        if (fullscreenToggle != null)
        {
            PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        }
        
        PlayerPrefs.Save();
    }
    
    private void OnVolumeChanged(float volume)
    {
        ApplyVolume(volume);
    }
    
    private void ApplyVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    
    private void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
    private void ReturnToPreviousMenu()
    {
        // Находим главное меню или меню паузы в сцене
        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        
        if (mainMenu != null)
        {
            mainMenu.Show();
        }
        else if (pauseMenu != null)
        {
            pauseMenu.Show();
        }
    }
    
    private void PlaySound()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.PlaySound(UIManager.Instance.buttonClickSound);
        }
    }
}