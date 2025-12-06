using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerController))] // Зависимость, так как мы его отключаем
public class PlayerCuteness : MonoBehaviour
{
    [Header("Cuteness Settings")]
    public float maxCuteness = 100f;
    public float startCuteness = 0f;

    [Header("Gameplay Balance")]
    public float passiveCutenessGain = 1f;

    [Header("Audio")]
    public AudioClip damageSound;   // Звук, когда милота растет (получаем "урон")
    public AudioClip restoreSound;  // Звук, когда убиваем врага (восстанавливаем брутальность)
    public AudioClip gameOverSound; // Звук проигрыша

    public float CurrentCuteness { get; private set; }

    private bool _isGameOver = false;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        CurrentCuteness = startCuteness;
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
        AddCuteness(passiveCutenessGain * Time.deltaTime, isPassive: true);
    }

    private void OnEnemyKilled(float brutailityRestoreAmount)
    {
        if (_isGameOver) return;

        RemoveCuteness(brutailityRestoreAmount);

        // Звук восстановления (приятный звук)
        if (restoreSound != null)
        {
            _audioSource.PlayOneShot(restoreSound);
        }
    }

    // Добавил параметр isPassive, чтобы звук не спамил каждую секунду от пассивного прироста
    public void AddCuteness(float amount, bool isPassive = false)
    {
        if (_isGameOver) return;

        CurrentCuteness += amount;
        CurrentCuteness = Mathf.Clamp(CurrentCuteness, 0, maxCuteness);

        // Играем звук "урона" (получения милоты) только если это не пассивное накопление
        if (!isPassive && damageSound != null && amount > 0)
        {
            _audioSource.PlayOneShot(damageSound);
        }

        CheckGameOver();
    }

    public void RemoveCuteness(float amount)
    {
        if (_isGameOver) return;

        CurrentCuteness -= amount;
        CurrentCuteness = Mathf.Clamp(CurrentCuteness, 0, maxCuteness);
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

        // Звук проигрыша
        if (gameOverSound != null)
        {
            _audioSource.PlayOneShot(gameOverSound);
        }

        GetComponent<PlayerController>().enabled = false;

        // Совет: Здесь лучше выключить музыку уровня или приглушить её, 
        // но это требует доступа к музыкальному менеджеру.
    }
}