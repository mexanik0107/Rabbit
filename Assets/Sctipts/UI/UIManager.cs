using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Аудио")]
    public AudioSource uiAudioSource;
    public AudioClip buttonClickSound;
    public AudioClip menuOpenSound;
    public AudioClip menuCloseSound;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // НЕ используем DontDestroyOnLoad - у каждой сцены свой UIManager
        }
    }
    
    public void PlaySound(AudioClip clip)
    {
        if (uiAudioSource != null && clip != null)
        {
            uiAudioSource.PlayOneShot(clip);
        }
    }
    
    // Общие методы для всех меню
    public void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
    
    public void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}