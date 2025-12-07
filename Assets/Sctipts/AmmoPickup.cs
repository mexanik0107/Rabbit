using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AmmoPickup : MonoBehaviour
{
    [Tooltip("Количество патронов в этой коробке")]
    public int ammoAmount = 10;

    [Tooltip("Звук подбора")]
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что в нас вошел именно Игрок
        if (collision.CompareTag("Player"))
        {
            // Пытаемся получить компонент стрельбы
            PlayerShooting shooting = collision.GetComponent<PlayerShooting>();

            if (shooting != null)
            {
                shooting.AddAmmo(ammoAmount);

                // Воспроизводим звук (через AudioSource.PlayClipAtPoint, т.к. объект сейчас уничтожится)
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}