using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    // Скрываем в инспекторе, так как устанавливаем эти значения из PlayerShooting
    [HideInInspector] public float speed;
    [HideInInspector] public float damage;

    private Rigidbody2D _rb;
    private float _lifetime = 5f; // Время жизни, чтобы пули не копились бесконечно

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Задаем начальную скорость по направлению "вверх" (локальная ось Y пули)
        _rb.linearVelocity = transform.up * speed;

        // Уничтожаем объект через _lifetime секунд
        Destroy(gameObject, _lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Пытаемся получить здоровье врага
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Уничтожаем пулю при попадании
        }
        // Уничтожаем пулю, если попали в стену (не игрок и не триггер)
        else if (!collision.CompareTag("Player") && !collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}