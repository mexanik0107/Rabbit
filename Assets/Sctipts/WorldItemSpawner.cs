using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldItemSpawner : MonoBehaviour
{
    [Header("Что спавним")]
    public GameObject itemPrefab; // Префаб коробки с патронами (AmmoPickup)

    [Header("Где спавним")]
    [Tooltip("Список точек, где могут появиться предметы. Можно перетащить сюда те же точки, что и в WaveManager.")]
    public List<Transform> spawnPoints;

    [Header("Настройки времени")]
    public float minSpawnInterval = 10f; // Мин. время между спавнами
    public float maxSpawnInterval = 20f; // Макс. время

    [Header("Настройки количества")]
    [Tooltip("Максимальное количество коробок, лежащих на карте одновременно. Чтобы не замусорить уровень.")]
    public int maxItemsOnMap = 5;

    [Header("Настройки лута")]
    public int minAmmoInBox = 5;
    public int maxAmmoInBox = 15;

    // Список для отслеживания созданных коробок
    private List<GameObject> _spawnedItems = new List<GameObject>();

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("WorldItemSpawner: Не назначены точки спавна!");
            enabled = false;
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogWarning("WorldItemSpawner: Не назначен префаб предмета!");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // Небольшая задержка перед первым спавном
        yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

        while (true)
        {
            // 1. Очистка списка (удаляем из него ссылки на коробки, которые игрок уже подобрал)
            // RemoveAll проверяет каждый элемент: если он null (уничтожен), удаляет из списка.
            _spawnedItems.RemoveAll(item => item == null);

            // 2. Проверка лимита
            if (_spawnedItems.Count < maxItemsOnMap)
            {
                SpawnItem();
            }

            // 3. Ждем до следующего раза
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnItem()
    {
        // Выбираем случайную точку
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Создаем предмет
        GameObject newItem = Instantiate(itemPrefab, randomPoint.position, Quaternion.identity);

        // Настраиваем количество патронов внутри
        AmmoPickup pickupScript = newItem.GetComponent<AmmoPickup>();
        if (pickupScript != null)
        {
            pickupScript.ammoAmount = Random.Range(minAmmoInBox, maxAmmoInBox + 1);
        }

        // Добавляем в список отслеживания
        _spawnedItems.Add(newItem);
    }
}