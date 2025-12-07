using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [Header("Loot Settings")]
    public GameObject ammoPickupPrefab; // Префаб коробки с патронами

    [Range(0f, 1f)]
    public float dropChance = 0.3f; // Шанс выпадения (0.3 = 30%)

    public int minAmmo = 5;
    public int maxAmmo = 15;

    // Этот метод мы вызовем из EnemyHealth перед смертью
    public void TryDropLoot()
    {
        if (ammoPickupPrefab == null) return;

        // Бросаем кубик (от 0.0 до 1.0)
        float roll = Random.value;

        if (roll <= dropChance)
        {
            // Определяем кол-во патронов
            int amount = Random.Range(minAmmo, maxAmmo + 1);

            // Создаем коробку на месте смерти врага
            GameObject loot = Instantiate(ammoPickupPrefab, transform.position, Quaternion.identity);

            // Настраиваем коробку (передаем в нее кол-во патронов)
            AmmoPickup pickupScript = loot.GetComponent<AmmoPickup>();
            if (pickupScript != null)
            {
                pickupScript.ammoAmount = amount;
            }
        }
    }
}