using UnityEngine;
using UnityEngine.Audio;

// RequireComponent гарантирует, что на объекте есть Collider2D (иначе триггер не сработает).
[RequireComponent(typeof(Collider2D))]
public class AmmoPickup : MonoBehaviour
{
    [Header("Настройки")]
    public int ammoAmount = 10; // Сколько патронов дает коробка

    [Header("Аудио")]
    public AudioClip pickupSound;     // Звук подбора
    public AudioMixerGroup sfxGroup;  // Группа микшера (чтобы громкость регулировалась в настройках)

    // Вызывается Unity, когда другой объект входит в триггер этого объекта
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что вошел именно игрок
        if (collision.CompareTag("Player"))
        {
            // Пытаемся получить компонент стрельбы у игрока
            PlayerShooting shooting = collision.GetComponent<PlayerShooting>();

            if (shooting != null)
            {
                // Добавляем патроны
                shooting.AddAmmo(ammoAmount);

                // Если есть звук, проигрываем его особым способом
                if (pickupSound != null)
                {
                    PlaySoundWithMixer(pickupSound, transform.position);
                }

                // Уничтожаем коробку с патронами (объект исчезает со сцены)
                Destroy(gameObject);
            }
        }
    }

    // Метод для создания временного источника звука.
    // Обычный AudioSource.PlayClipAtPoint не поддерживает AudioMixerGroup, поэтому пишем свой велосипед.
    private void PlaySoundWithMixer(AudioClip clip, Vector3 position)
    {
        // Создаем пустой объект
        GameObject tempAudio = new GameObject("AmmoPickupSound");
        tempAudio.transform.position = position;

        // Добавляем AudioSource и настраиваем его
        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = clip;

        // Важно: назначаем канал микшера (SFX)
        if (sfxGroup != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
        }

        source.Play();

        // Уничтожаем объект звука, когда клип закончится
        Destroy(tempAudio, clip.length);
    }
}