using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerController))]
public class PlayerCuteness : MonoBehaviour
{
    [Header("Cuteness Settings")]
    public float maxCuteness = 100f;
    public float startCuteness = 0f;

    [Header("Gameplay Balance")]
    public float passiveCutenessGain = 1f; // Пассивное накопление милоты со временем

    [Header("UI References")]
    public HUD hud; // Ссылка на HUD (нужно перетащить в инспекторе)

    [Header("Audio")]
    public AudioClip damageSound;   // Звук получения милоты
    public AudioClip restoreSound;  // Звук восстановления (убийство врага)
    public AudioClip gameOverSound; // Звук Game Over

    public float CurrentCuteness { get; private set; }

    private bool _isGameOver = false;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        // Если HUD не назначен в инспекторе, пробуем найти его на сцене
        if (hud == null)
        {
            hud = FindObjectOfType<HUD>();
        }
    }

    void Start()
    {
        CurrentCuteness = startCuteness;

        // Инициализируем HUD значениями
        if (hud != null)
        {
            hud.Initialize(maxCuteness, CurrentCuteness);
        }
    }

    void OnEnable()
    {
        EnemyHealth.OnEnemyDied += OnEnemyKilled;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= OnEnemyKilled;
    }

    void Update()
    {
        if (_isGameOver) return;

        // Пассивное накопление милоты (это усложняет игру со временем)
        if (passiveCutenessGain > 0)
        {
            AddCuteness(passiveCutenessGain * Time.deltaTime, isPassive: true);
        }
    }

    private void OnEnemyKilled(float brutailityRestoreAmount)
    {
        if (_isGameOver) return;

        RemoveCuteness(brutailityRestoreAmount);

        if (restoreSound != null)
        {
            _audioSource.PlayOneShot(restoreSound);
        }
    }

    // Добавляем милоту (аналог получения урона)
    public void AddCuteness(float amount, bool isPassive = false)
    {
        if (_isGameOver) return;

        CurrentCuteness += amount;
        CurrentCuteness = Mathf.Clamp(CurrentCuteness, 0, maxCuteness);

        // Обновляем UI
        if (hud != null)
        {
            hud.UpdateCuteness(CurrentCuteness);
        }

        // Звук играем только если это удар врага, а не пассивный тик
        if (!isPassive && damageSound != null && amount > 0)
        {
            _audioSource.PlayOneShot(damageSound);
        }

        CheckGameOver();
    }

    // Убираем милоту (лечимся, убивая врагов)
    public void RemoveCuteness(float amount)
    {
        if (_isGameOver) return;

        CurrentCuteness -= amount;
        CurrentCuteness = Mathf.Clamp(CurrentCuteness, 0, maxCuteness);

        // Обновляем UI
        if (hud != null)
        {
            hud.UpdateCuteness(CurrentCuteness);
        }
    }

    public float GetCutenessNormalized()
    {
        return CurrentCuteness / maxCuteness;
    }

    private void CheckGameOver()
    {
        if (CurrentCuteness >= maxCuteness)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        _isGameOver = true;
        Debug.Log("GAME OVER: Вы превратились в зайчика! Слишком мило!");

        if (gameOverSound != null)
        {
            _audioSource.PlayOneShot(gameOverSound);
        }

        GetComponent<PlayerController>().enabled = false;

        // Здесь можно вызвать экран проигрыша через UIManager, если он есть
        // Example: UIManager.Instance.ShowGameOverScreen();
    }
}