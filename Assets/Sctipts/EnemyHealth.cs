using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 10f;

    [Header("Audio")]
    public AudioClip hitSound;  // Звук попадания по врагу
    public AudioClip deathSound; // Звук смерти врага

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

        // Звук получения урона
        if (hitSound != null)
        {
            // Используем PlayOneShot, чтобы звуки могли накладываться (например, очередь из автомата)
            _audioSource.PlayOneShot(hitSound);
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnEnemyDied?.Invoke(5f);

        // ВАЖНО: Используем PlayClipAtPoint, так как gameObject сейчас будет уничтожен.
        // Если использовать _audioSource.Play(), звук прервется мгновенно.
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        Destroy(gameObject);
    }
}