using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class PlayerCuteness : MonoBehaviour
{
    [Header("Settings")]
    public float maxCuteness = 100f;
    public float startCuteness = 0f;

    [Header("Combat Balance")]
    [Range(0f, 100f)]
    public float killReductionPercent = 10f;

    [Header("References")]
    public HUD hud;

    [Header("Audio")]
    // Перетащи сюда группу SFX
    public AudioMixerGroup sfxGroup;
    public AudioClip hitSound;
    public AudioClip healSound;
    public AudioClip gameOverSound;

    private float _currentCuteness;
    private AudioSource _audioSource;
    private bool _isDead = false;

    public float CurrentCuteness => _currentCuteness;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        // Привязка к микшеру
        if (sfxGroup != null && _audioSource != null)
        {
            _audioSource.outputAudioMixerGroup = sfxGroup;
        }

        if (hud == null) hud = FindObjectOfType<HUD>();
    }

    void OnEnable()
    {
        EnemyHealth.OnEnemyDied += HandleEnemyDeath;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDeath;
    }

    void Start()
    {
        _currentCuteness = startCuteness;
        if (hud != null) hud.Initialize(maxCuteness, _currentCuteness);
    }

    private void HandleEnemyDeath(float points)
    {
        if (_isDead) return;
        float reductionAmount = maxCuteness * (killReductionPercent / 100f);
        RemoveCuteness(reductionAmount);
    }

    public void AddCuteness(float amount, bool playSound = true)
    {
        if (_isDead) return;

        _currentCuteness += amount;

        if (playSound && amount > 1f && hitSound != null)
            _audioSource.PlayOneShot(hitSound);

        UpdateHUD();

        if (_currentCuteness >= maxCuteness)
        {
            Die();
        }
    }

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
            // Используем кастомный метод, так как объект игрока может отключиться
            PlaySoundIndependent(gameOverSound);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

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