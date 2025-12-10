using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Нужно для отслеживания наведения мыши

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
{
    // Если хочешь для конкретной кнопки свой звук, назначь сюда.
    // Если пусто — будет стандартный "Щелк" из UIManager.
    public AudioClip customClickSound;
    public bool playHoverSound = true;

    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();

        // Подписываемся на клик кнопки
        _button.onClick.AddListener(PlayClick);
    }

    private void PlayClick()
    {
        if (UIManager.Instance == null) return;

        // Выбираем звук: либо кастомный, либо стандартный из менеджера
        AudioClip clipToPlay = customClickSound != null ? customClickSound : UIManager.Instance.buttonClickSound;

        UIManager.Instance.PlaySound(clipToPlay);
    }

    // Этот метод срабатывает, когда мышка наводится на кнопку
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playHoverSound || _button.interactable == false) return;

        // Обычно для ховера звук тихий, можно использовать тот же click или другой
        // Если хочешь отдельный звук ховера, добавь его в UIManager
        // Пока просто не будем играть ничего или (опционально) тот же клик
    }
}