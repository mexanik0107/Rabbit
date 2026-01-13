using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [Header("Настройки лута")]
    public GameObject ammoPickupPrefab; // Что выпадает (коробка патронов)

    [Range(0f, 1f)]
    public float dropChance = 0.3f; // Вероятность выпадения (0.3 = 30%)

    public int minAmmo = 5;  // Мин. патронов в коробке
    public int maxAmmo = 15; // Макс. патронов в коробке

    // Этот метод вызывается извне (обычно скриптом EnemyHealth при смерти)
    public void TryDropLoot()
    {
        if (ammoPickupPrefab == null) return;

        // Random.value возвращает число от 0.0 до 1.0.
        // Если выпало число меньше нашего шанса — лут создается.
        float roll = Random.value;

        if (roll <= dropChance)
        {
            // Рассчитываем случайное количество патронов
            int amount = Random.Range(minAmmo, maxAmmo + 1);

            // Instantiate создает копию префаба в мире (на месте врага)
            GameObject loot = Instantiate(ammoPickupPrefab, transform.position, Quaternion.identity);

            // Находим скрипт на созданном объекте и передаем туда количество патронов
            AmmoPickup pickupScript = loot.GetComponent<AmmoPickup>();
            if (pickupScript != null)
            {
                pickupScript.ammoAmount = amount;
            }
        }
    }
}