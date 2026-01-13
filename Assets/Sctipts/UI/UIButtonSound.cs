using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
{
    // Опциональный кастомный звук для уникальных кнопок
    public AudioClip customClickSound;
    public bool playHoverSound = true;

    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlayClick);
    }

    private void PlayClick()
    {
        if (UIManager.Instance == null) return;

        // Если кастомный звук не назначен, берем стандартный из UIManager
        AudioClip clipToPlay = customClickSound != null ? customClickSound : UIManager.Instance.buttonClickSound;

        UIManager.Instance.PlaySound(clipToPlay);
    }

    // Событие наведения мыши
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playHoverSound || _button.interactable == false) return;
        // Здесь можно добавить звук наведения (Hover), если нужно
    }
}