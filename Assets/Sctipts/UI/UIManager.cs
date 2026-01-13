using UnityEngine;
using UnityEngine.Audio;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Аудио")]
    public AudioMixerGroup sfxGroup;
    public AudioSource uiAudioSource;
    public AudioClip buttonClickSound;
    public AudioClip menuOpenSound;
    public AudioClip menuCloseSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (sfxGroup != null && uiAudioSource != null)
        {
            uiAudioSource.outputAudioMixerGroup = sfxGroup;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (uiAudioSource != null && clip != null)
        {
            uiAudioSource.PlayOneShot(clip);
        }
    }

    // Методы для удобного переключения CanvasGroup
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