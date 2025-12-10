using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Screen Settings")]
    public Button fullscreenBtn;
    public Button windowedBtn;
    public Button backButton;

    [Header("Visuals")]
    public Image fullscreenBtnImage;
    public Image windowedBtnImage;
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private System.Action _onBackCallback;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        InitializeSettings();

        // Подписки на события
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Для кнопок добавляем звуки и логику
        SetupButton(fullscreenBtn, () => SetFullscreen(true));
        SetupButton(windowedBtn, () => SetFullscreen(false));
        SetupButton(backButton, CloseSettings);

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    // Вспомогательный метод чтобы вешать и логику, и звук
    private void SetupButton(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (btn != null)
        {
            btn.onClick.AddListener(action);
            // Автоматически вешаем скрипт звука, если его нет
            if (btn.GetComponent<UIButtonSound>() == null)
                btn.gameObject.AddComponent<UIButtonSound>();
        }
    }

    private void Start()
    {
        ApplyCurrentSettings();
    }

    private void InitializeSettings()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);

        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        bool isFull = PlayerPrefs.GetInt("FullscreenState", Screen.fullScreen ? 1 : 0) == 1;
        UpdateScreenButtons(isFull);
    }

    private void ApplyCurrentSettings()
    {
        if (musicSlider != null) SetMusicVolume(musicSlider.value);
        if (sfxSlider != null) SetSFXVolume(sfxSlider.value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVol", value);
        if (mainMixer == null) return;
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("MusicVol", dB);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVol", value);
        if (mainMixer == null) return;
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("SFXVol", dB);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenState", isFullscreen ? 1 : 0);
        UpdateScreenButtons(isFullscreen);
    }

    private void UpdateScreenButtons(bool isFullscreen)
    {
        if (activeSprite != null && inactiveSprite != null)
        {
            if (fullscreenBtnImage != null) fullscreenBtnImage.sprite = isFullscreen ? activeSprite : inactiveSprite;
            if (windowedBtnImage != null) windowedBtnImage.sprite = !isFullscreen ? activeSprite : inactiveSprite;
        }
    }

    public void Open(System.Action onBackAction)
    {
        _onBackCallback = onBackAction;
        gameObject.SetActive(true);
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        ApplyCurrentSettings();
    }

    public void CloseSettings()
    {
        // --- ЗВУК ЗАКРЫТИЯ ---
        if (UIManager.Instance != null)
            UIManager.Instance.PlaySound(UIManager.Instance.menuCloseSound);
        // ---------------------

        PlayerPrefs.Save();
        gameObject.SetActive(false);
        _onBackCallback?.Invoke();
    }
}