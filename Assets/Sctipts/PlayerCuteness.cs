using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class PlayerCuteness : MonoBehaviour
{
    [Header("Настройки")]
    public float maxCuteness = 100f;
    public float startCuteness = 0f;

    [Header("Баланс")]
    [Range(0f, 100f)]
    public float killReductionPercent = 10f; // На сколько процентов падает милота при убийстве

    [Header("Ссылки")]
    public HUD hud;

    [Header("Аудио")]
    public AudioMixerGroup sfxGroup;
    public AudioClip hitSound;      // Звук получения "урона" (милоты)
    public AudioClip healSound;     // Звук снижения милоты (как бы лечение)
    public AudioClip gameOverSound;

    private float _currentCuteness;
    private AudioSource _audioSource;
    private bool _isDead = false;

    // Публичное свойство для чтения текущего значения (инкапсуляция)
    public float CurrentCuteness => _currentCuteness;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (sfxGroup != null && _audioSource != null)
        {
            _audioSource.outputAudioMixerGroup = sfxGroup;
        }

        if (hud == null) hud = FindObjectOfType<HUD>();
    }

    // Подписываемся на события при включении объекта
    void OnEnable()
    {
        EnemyHealth.OnEnemyDied += HandleEnemyDeath;
    }

    // Отписываемся при выключении (важно, чтобы избежать утечек памяти)
    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDeath;
    }

    void Start()
    {
        _currentCuteness = startCuteness;
        if (hud != null) hud.Initialize(maxCuteness, _currentCuteness);
    }

    // Метод-обработчик события "Враг умер"
    private void HandleEnemyDeath(float points)
    {
        if (_isDead) return;
        float reductionAmount = maxCuteness * (killReductionPercent / 100f);
        RemoveCuteness(reductionAmount);
    }

    // "Получение урона" (милота растет)
    public void AddCuteness(float amount, bool playSound = true)
    {
        if (_isDead) return;

        _currentCuteness += amount;

        if (playSound && amount > 1f && hitSound != null)
            _audioSource.PlayOneShot(hitSound);

        UpdateHUD();

        // Если милота зашкалила — проигрыш
        if (_currentCuteness >= maxCuteness)
        {
            Die();
        }
    }

    // "Лечение" (милота падает)
    public void RemoveCuteness(float amount)
    {
        if (_isDead) return;
        bool wasAlreadySafe = _currentCuteness <= 0.01f;

        _currentCuteness -= amount;
        if (_currentCuteness < 0) _currentCuteness = 0;

        if (!wasAlreadySafe && amount > 0 && healSound != null)
            _audioSource.PlayOneShot(healSound);

        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (hud != null) hud.UpdateCuteness(_currentCuteness);
    }

    private void Die()
    {
        _isDead = true;

        if (gameOverSound != null)
        {
            PlaySoundIndependent(gameOverSound);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            // Фолбэк: перезагрузка сцены, если нет ГеймМенеджера
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    // Создаем независимый звук, так как объект игрока может быть отключен при смерти
    private void PlaySoundIndependent(AudioClip clip)
    {
        GameObject go = new GameObject("GameOverSound");
        go.transform.position = transform.position;
        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;
        if (sfxGroup != null) src.outputAudioMixerGroup = sfxGroup;
        src.Play();
        Destroy(go, clip.length);
    }
}