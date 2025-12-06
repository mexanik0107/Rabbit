using Mono.Cecil.Cil;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
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
    public float cutenessDamage = 10f; // Сколько "Милоты" добавляет враг при ударе

    private Rigidbody2D _rb;
    private float _lastAttackTime;
    private Vector2 _movementVector;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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

        // === ИЗМЕНЕНИЕ: Взаимодействие с PlayerCuteness ===
        if (player != null)
        {
            PlayerCuteness playerCuteness = player.GetComponent<PlayerCuteness>();
            if (playerCuteness != null)
            {
                // "Атака" врага увеличивает шкалу милоты игрока
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