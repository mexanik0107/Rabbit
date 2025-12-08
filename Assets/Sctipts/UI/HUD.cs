using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Индикатор Милоты")]
    public Slider cutenessSlider;
    public Image cutenessFill;
    [Tooltip("Градиент цвета: Слева (0%) - Безопасно, Справа (100%) - Опасно")]
    public Gradient cutenessGradient;

    [Header("Дополнительная информация")]
    public Text scoreText;
    public Text ammoText;
    public Text waveText;

    [Header("Анимация получения милоты")]
    public Animator damageAnimator; // Анимация тряски или вспышки при попадании

    private float _maxCuteness;

    // Инициализация при старте игры
    public void Initialize(float maxCutenessValue, float currentCutenessValue)
    {
        _maxCuteness = maxCutenessValue;

        if (cutenessSlider != null)
        {
            cutenessSlider.maxValue = _maxCuteness;
            cutenessSlider.value = currentCutenessValue;
            UpdateCutenessColor(currentCutenessValue);
        }
    }

    // Метод обновления полоски (вызывается из PlayerCuteness)
    public void UpdateCuteness(float currentCuteness)
    {
        // Ограничиваем значение для UI
        float clampedValue = Mathf.Clamp(currentCuteness, 0, _maxCuteness);

        // Запоминаем старое значение, чтобы понять, выросла ли милота
        float oldValue = cutenessSlider != null ? cutenessSlider.value : 0;

        if (cutenessSlider != null)
        {
            cutenessSlider.value = clampedValue;
            UpdateCutenessColor(clampedValue);
        }

        // Если милота выросла (нас ударили) -> проигрываем анимацию "урона"
        if (damageAnimator != null && clampedValue > oldValue)
        {
            damageAnimator.SetTrigger("TakeDamage");
        }
    }

    private void UpdateCutenessColor(float currentValue)
    {
        if (cutenessFill != null && cutenessGradient != null)
        {
            float percentage = currentValue / _maxCuteness;
            cutenessFill.color = cutenessGradient.Evaluate(percentage);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // --- Дополнительные методы ---

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Очки: {score}";
        }
    }

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Патроны: {currentAmmo}/{maxAmmo}";
        }
    }

    public void UpdateWave(int waveNumber)
    {
        if (waveText != null)
        {
            waveText.text = $"Волна: {waveNumber}";
        }
    }
}