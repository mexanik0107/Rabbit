using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAI : MonoBehaviour
{
    [Header("Targeting")]
    public Transform player;
    public bool findPlayerAutomatically = true;

    [Header("Jump Movement Settings")]
    public float jumpSpeed = 6f;
    public float jumpDuration = 0.6f;
    public float jumpInterval = 1f;
    public AnimationCurve jumpScaleCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.5f), new Keyframe(1, 0));

    [Header("Visuals & Animation")]
    [Range(0.1f, 3f)] public float sizeMultiplier = 1.0f;
    public float animationFps = 10f;
    public Sprite[] idleSprites;
    public Sprite[] jumpSprites;

    [Header("Stats")]
    public float rotationSpeed = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float cutenessDamage = 10f;

    [Header("Audio")]
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

        // Привязываем к микшеру
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

        StartCoroutine(RabbitBehaviorRoutine());
        StartCoroutine(AnimationRoutine());
    }

    void Update()
    {
        HandleAttackLogic();
    }

    private IEnumerator AnimationRoutine()
    {
        int frameIndex = 0;
        float timer = 0f;

        while (true)
        {
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

        // --- ИЗМЕНЕНИЕ: Звук атаки отключен ---
        // if (attackSound != null) _audioSource.PlayOneShot(attackSound);
        // --------------------------------------

        if (player != null)
        {
            PlayerCuteness playerCuteness = player.GetComponent<PlayerCuteness>();
            if (playerCuteness != null)
            {
                playerCuteness.AddCuteness(cutenessDamage);
            }
        }
    }

    private IEnumerator RabbitBehaviorRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f));

        while (true)
        {
            _isJumping = false;
            float timer = 0f;
            while (timer < jumpInterval)
            {
                RotateTowardsPlayer();
                transform.localScale = GetCurrentTargetSize();
                timer += Time.deltaTime;
                yield return null;
            }

            if (player != null)
            {
                yield return StartCoroutine(PerformJump());
            }
        }
    }

    private IEnumerator PerformJump()
    {
        _isJumping = true;
        Vector2 jumpDirection = (player.position - transform.position).normalized;

        // Звук прыжка остался
        if (jumpSound != null) _audioSource.PlayOneShot(jumpSound);

        float timeElapsed = 0f;
        Vector3 startJumpSize = GetCurrentTargetSize();

        while (timeElapsed < jumpDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / jumpDuration;
            _rb.MovePosition(_rb.position + jumpDirection * jumpSpeed * Time.fixedDeltaTime);
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
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private Vector3 GetCurrentTargetSize()
    {
        return _baseEditorScale * sizeMultiplier;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}