using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Cuteness Bar")]
    public Slider cutenessSlider;
    public Image cutenessFill;
    public Gradient cutenessGradient;

    [Header("Ammo Info")]
    public TMP_Text currentAmmoText;
    public TMP_Text totalAmmoText;

    [Header("Game Info")]
    public TMP_Text waveText;       // Сюда будет писаться "WAVE 1" или "NEXT: 5..."
    public TMP_Text gameTimerText;  // НОВОЕ: Сюда перетащи текст общего таймера

    [Header("Visual Darkening")]
    public Gradient interfaceGradient;
    public Image[] allHudImages;
    public TMP_Text[] allHudTexts;

    private float _maxCuteness;

    public void Initialize(float maxCutenessValue, float currentCutenessValue)
    {
        _maxCuteness = maxCutenessValue;
        if (cutenessSlider != null)
        {
            cutenessSlider.maxValue = _maxCuteness;
            cutenessSlider.value = currentCutenessValue;
        }
        UpdateCutenessVisuals(currentCutenessValue);
    }

    public void UpdateCuteness(float currentCuteness)
    {
        if (cutenessSlider != null) cutenessSlider.value = currentCuteness;
        UpdateCutenessVisuals(currentCuteness);
    }

    private void UpdateCutenessVisuals(float currentVal)
    {
        float percentage = Mathf.Clamp01(currentVal / _maxCuteness);

        // 1. Полоска (полный цвет)
        if (cutenessFill != null) cutenessFill.color = cutenessGradient.Evaluate(percentage);

        // 2. Цвет интерфейса
        Color targetColor = interfaceGradient.Evaluate(percentage);

        // Картинки красим как обычно
        if (allHudImages != null)
        {
            foreach (var img in allHudImages)
            {
                if (img != null && img != cutenessFill) img.color = targetColor;
            }
        }

        // ТЕКСТ: Красим только ЛИЦЕВУЮ часть (faceColor), чтобы Аутлайн остался черным!
        if (allHudTexts != null)
        {
            foreach (var txt in allHudTexts)
            {
                if (txt != null)
                {
                    // faceColor принимает Color32, приведение автоматическое
                    txt.faceColor = targetColor;
                }
            }
        }
    }

    // --- Обновление информации ---

    public void UpdateAmmo(int clip, int total)
    {
        if (currentAmmoText != null) currentAmmoText.text = clip.ToString();
        if (totalAmmoText != null) totalAmmoText.text = total.ToString();
    }

    public void UpdateWaveText(string text)
    {
        if (waveText != null) waveText.text = text;
    }

    public void UpdateGameTimer(float timeInSeconds)
    {
        if (gameTimerText != null)
        {
            // Форматируем время в ММ:СС
            int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
            int seconds = Mathf.FloorToInt(timeInSeconds - minutes * 60);
            gameTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}   