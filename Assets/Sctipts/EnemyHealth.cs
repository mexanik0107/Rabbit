using UnityEngine;
using System;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Настройки")]
    public float maxHealth = 10f;

    [Header("Аудио")]
    public AudioMixerGroup sfxGroup;
    public AudioClip hitSound;
    public AudioClip deathSound;

    // Статическое событие, на которое может подписаться кто угодно (например, WaveManager или PlayerCuteness)
    public static event Action<float> OnEnemyDied;

    private float _currentHealth;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

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
        // Проверяем, есть ли компонент для выпадения лута
        if (TryGetComponent(out LootDropper looter))
        {
            looter.TryDropLoot();
        }

        // Сообщаем всем подписчикам, что враг умер (передаем очки, например 5)
        OnEnemyDied?.Invoke(5f);

        if (deathSound != null)
        {
            // Используем PlaySoundAndDestroy, так как этот объект (gameObject) сейчас будет уничтожен,
            // и обычный AudioSource прервется.
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