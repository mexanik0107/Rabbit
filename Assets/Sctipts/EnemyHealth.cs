using UnityEngine;
using System;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 10f;

    [Header("Audio")]
    // Перетащи сюда группу SFX в префабе врага
    public AudioMixerGroup sfxGroup;
    public AudioClip hitSound;
    public AudioClip deathSound;

    public static event Action<float> OnEnemyDied;

    private float _currentHealth;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        // Привязываем основной источник звука (для получения урона) к микшеру
        if (sfxGroup != null && _audioSource != null)
        {
            _audioSource.outputAudioMixerGroup = sfxGroup;
        }
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
        if (TryGetComponent(out LootDropper looter))
        {
            looter.TryDropLoot();
        }

        OnEnemyDied?.Invoke(5f);

        if (deathSound != null)
        {
            // Создаем временный звук с привязкой к микшеру
            PlaySoundAndDestroy(deathSound, transform.position);
        }

        Destroy(gameObject);
    }

    private void PlaySoundAndDestroy(AudioClip clip, Vector3 position)
    {
        GameObject go = new GameObject("DeathSound");
        go.transform.position = position;
        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;

        if (sfxGroup != null)
        {
            src.outputAudioMixerGroup = sfxGroup;
        }

        src.Play();
        Destroy(go, clip.length);
    }
}