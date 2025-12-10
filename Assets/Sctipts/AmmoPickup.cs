using UnityEngine;
using UnityEngine.Audio; // ВАЖНО

[RequireComponent(typeof(Collider2D))]
public class AmmoPickup : MonoBehaviour
{
    [Header("Settings")]
    public int ammoAmount = 10;

    [Header("Audio")]
    public AudioClip pickupSound;
    // Перетащи сюда группу SFX
    public AudioMixerGroup sfxGroup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerShooting shooting = collision.GetComponent<PlayerShooting>();

            if (shooting != null)
            {
                shooting.AddAmmo(ammoAmount);

                if (pickupSound != null)
                {
                    // Вместо PlayClipAtPoint используем свой метод с микшером
                    PlaySoundWithMixer(pickupSound, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }

    private void PlaySoundWithMixer(AudioClip clip, Vector3 position)
    {
        // Создаем временный объект для звука
        GameObject tempAudio = new GameObject("AmmoPickupSound");
        tempAudio.transform.position = position;

        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = clip;

        // --- САМОЕ ГЛАВНОЕ: Назначаем группу микшера ---
        if (sfxGroup != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
        }

        source.Play();
        Destroy(tempAudio, clip.length);
    }
}