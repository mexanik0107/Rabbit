using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))] // Врагу тоже нужен источник звука
public class EnemyAI : MonoBehaviour
{
    [Header("Targeting")]
    public Transform player;
    public bool findPlayerAutomatically = true;

    [Header("Movement Stats")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;

    [Header("Combat Stats")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float cutenessDamage = 10f;

    [Header("Audio")]
    public AudioClip attackSound; // Звук "кусь" или удара

    private Rigidbody2D _rb;
    private AudioSource _audioSource;
    private float _lastAttackTime;
    private Vector2 _movementVector;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (findPlayerAutomatically && player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            CalculateMovement();
        }
        else
        {
            StopMovement();
            if (Time.time >= _lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
        }

        RotateTowardsPlayer();
    }

    void FixedUpdate()
    {
        MoveEnemy();
    }

    private void CalculateMovement()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        _movementVector = direction;
    }

    private void StopMovement()
    {
        _movementVector = Vector2.zero;
    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void PerformAttack()
    {
        _lastAttackTime = Time.time;

        // Воспроизводим звук атаки
        if (attackSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(attackSound);
        }

        if (player != null)
        {
            PlayerCuteness playerCuteness = player.GetComponent<PlayerCuteness>();
            if (playerCuteness != null)
            {
                playerCuteness.AddCuteness(cutenessDamage);
                Debug.Log($"{name} добавил игроку милоты!");
            }
        }
    }

    private void MoveEnemy()
    {
        _rb.MovePosition(_rb.position + _movementVector * moveSpeed * Time.fixedDeltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}