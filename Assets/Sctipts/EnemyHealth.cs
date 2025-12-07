using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 10f;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip deathSound;

    public static event Action<float> OnEnemyDied;

    private float _currentHealth;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if (hitSound != null)
        {
            _audioSource.PlayOneShot(hitSound);
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // --- НОВОЕ: Проверка на лут ---
        // Пытаемся найти компонент LootDropper на этом же враге
        // Если он есть, вызываем метод TryDropLoot()
        if (TryGetComponent(out LootDropper looter))
        {
            looter.TryDropLoot();
        }
        // ------------------------------

        OnEnemyDied?.Invoke(5f);

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        Destroy(gameObject);
    }
}