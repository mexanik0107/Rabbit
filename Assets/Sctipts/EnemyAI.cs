using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAI : MonoBehaviour
{
    [Header("Поиск цели")]
    public Transform player;
    public bool findPlayerAutomatically = true;

    [Header("Прыжки")]
    public float jumpSpeed = 6f;
    public float jumpDuration = 0.6f;
    public float jumpInterval = 1f; // Пауза между прыжками
    // Кривая для анимации "сжатия/растяжения" при прыжке
    public AnimationCurve jumpScaleCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.5f), new Keyframe(1, 0));

    [Header("Визуал")]
    [Range(0.1f, 3f)] public float sizeMultiplier = 1.0f;
    public float animationFps = 10f;
    public Sprite[] idleSprites; // Спрайты покоя
    public Sprite[] jumpSprites; // Спрайты прыжка

    [Header("Параметры")]
    public float rotationSpeed = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float cutenessDamage = 10f; // Урон милотой

    [Header("Аудио")]
    public AudioMixerGroup sfxGroup;
    public AudioClip attackSound;
    public AudioClip jumpSound;

    private Rigidbody2D _rb;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private float _lastAttackTime;
    private Vector3 _baseEditorScale;
    private bool _isJumping = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (sfxGroup != null && _audioSource != null)
        {
            _audioSource.outputAudioMixerGroup = sfxGroup;
        }

        _baseEditorScale = transform.localScale;
    }

    void Start()
    {
        if (findPlayerAutomatically && player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // Запускаем независимые корутины для поведения и анимации
        StartCoroutine(RabbitBehaviorRoutine());
        StartCoroutine(AnimationRoutine());
    }

    void Update()
    {
        HandleAttackLogic();
    }

    // Корутина для покадровой анимации (меняет спрайты)
    private IEnumerator AnimationRoutine()
    {
        int frameIndex = 0;
        float timer = 0f;

        while (true)
        {
            // Выбираем массив спрайтов в зависимости от состояния
            Sprite[] currentClips = _isJumping ? jumpSprites : idleSprites;

            if (currentClips != null && currentClips.Length > 0)
            {
                float frameDuration = 1f / animationFps;
                timer += Time.deltaTime;

                if (timer >= frameDuration)
                {
                    timer = 0f;
                    frameIndex++;
                    if (frameIndex >= currentClips.Length) frameIndex = 0;
                    _spriteRenderer.sprite = currentClips[frameIndex];
                }
            }
            yield return null;
        }
    }

    private void HandleAttackLogic()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= _lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
        }
    }

    private void PerformAttack()
    {
        _lastAttackTime = Time.time;

        if (attackSound != null) _audioSource.PlayOneShot(attackSound);

        if (player != null)
        {
            PlayerCuteness playerCuteness = player.GetComponent<PlayerCuteness>();
            if (playerCuteness != null)
            {
                // Наносим "урон" милотой
                playerCuteness.AddCuteness(cutenessDamage);
            }
        }
    }

    // Основной цикл поведения AI
    private IEnumerator RabbitBehaviorRoutine()
    {
        // Случайная задержка на старте, чтобы враги не прыгали синхронно
        yield return new WaitForSeconds(Random.Range(0f, 1f));

        while (true)
        {
            _isJumping = false;

            // Фаза подготовки (вращение к игроку)
            float timer = 0f;
            while (timer < jumpInterval)
            {
                RotateTowardsPlayer();
                transform.localScale = GetCurrentTargetSize(); // Сброс скейла
                timer += Time.deltaTime;
                yield return null;
            }

            // Фаза прыжка
            if (player != null)
            {
                yield return StartCoroutine(PerformJump());
            }
        }
    }

    private IEnumerator PerformJump()
    {
        _isJumping = true;

        // Прыгаем в направлении, где был игрок в начале прыжка
        Vector2 jumpDirection = (player.position - transform.position).normalized;

        if (jumpSound != null) _audioSource.PlayOneShot(jumpSound);

        float timeElapsed = 0f;
        Vector3 startJumpSize = GetCurrentTargetSize();

        while (timeElapsed < jumpDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / jumpDuration;

            // Физическое перемещение
            _rb.MovePosition(_rb.position + jumpDirection * jumpSpeed * Time.fixedDeltaTime);

            // Визуальный эффект "желе" через AnimationCurve
            float curveValue = jumpScaleCurve.Evaluate(t);
            transform.localScale = startJumpSize * (1f + curveValue);

            yield return new WaitForFixedUpdate();
        }

        transform.localScale = GetCurrentTargetSize();
        _isJumping = false;
    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;
        Vector2 direction = player.position - transform.position;
        // -90 нужно, если спрайт врага смотрит вверх по умолчанию
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private Vector3 GetCurrentTargetSize()
    {
        return _baseEditorScale * sizeMultiplier;
    }

    // Рисуем радиус атаки в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}