using UnityEngine;
using System; // Нужно для Action
public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 10f;
// СОБЫТИЕ: На него подпишется игрок, система очков и спаунер волн
// "static" означает, что событие общее для всех врагов
    public static event Action<float> OnEnemyDied;

    private float _currentHealth;

    void OnEnable()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        // Эффект попадания (мигание или частицы) можно вызвать здесь
        // Но пока оставим логику чистой

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Вызываем событие "Враг умер". 
        // Передаем значение (например, сколько "брутальности" восстановит убийство)
        // 5f - это просто пример значения
        OnEnemyDied?.Invoke(5f);

        // Уничтожаем объект врага
        Destroy(gameObject);
    }
}