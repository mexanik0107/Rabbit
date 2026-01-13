using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Полоска Милоты")]
    public Slider cutenessSlider;
    public Image cutenessFill;
    public Gradient cutenessGradient; // Позволяет менять цвет полоски от зеленого к красному

    [Header("Патроны")]
    public TMP_Text currentAmmoText;
    public TMP_Text totalAmmoText;

    [Header("Инфо")]
    public TMP_Text waveText;
    public TMP_Text gameTimerText;

    [Header("Визуальное затемнение")]
    // Градиент для изменения цвета всего интерфейса при повышении опасности
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

        // 1. Меняем цвет самой полоски (Health Bar)
        if (cutenessFill != null) cutenessFill.color = cutenessGradient.Evaluate(percentage);

        // 2. Меняем цвет элементов интерфейса (для атмосферы)
        Color targetColor = interfaceGradient.Evaluate(percentage);

        if (allHudImages != null)
        {
            foreach (var img in allHudImages)
            {
                if (img != null && img != cutenessFill) img.color = targetColor;
            }
        }

        if (allHudTexts != null)
        {
            foreach (var txt in allHudTexts)
            {
                if (txt != null)
                {
                    // Меняем только цвет заливки текста (faceColor), чтобы обводка осталась черной
                    txt.faceColor = targetColor;
                }
            }
        }
    }

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
            int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
            int seconds = Mathf.FloorToInt(timeInSeconds - minutes * 60);
            gameTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}