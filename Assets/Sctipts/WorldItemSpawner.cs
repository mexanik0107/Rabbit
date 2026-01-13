using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldItemSpawner : MonoBehaviour
{
    [Header("Что спавним")]
    public GameObject itemPrefab; // Префаб (например, аптечка или патроны)

    [Header("Где спавним")]
    public List<Transform> spawnPoints; // Список точек на карте

    [Header("Настройки времени")]
    public float minSpawnInterval = 10f;
    public float maxSpawnInterval = 20f;

    [Header("Настройки количества")]
    public int maxItemsOnMap = 5; // Лимит, чтобы не завалить всю карту предметами

    [Header("Настройки лута")]
    public int minAmmoInBox = 5;
    public int maxAmmoInBox = 15;

    // Список для хранения ссылок на уже созданные предметы
    private List<GameObject> _spawnedItems = new List<GameObject>();

    void Start()
    {
        // Проверки на ошибки конфигурации
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

        // Запускаем бесконечный процесс спавна
        StartCoroutine(SpawnRoutine());
    }

    // Корутина — это метод, выполнение которого можно приостанавливать (yield return).
    private IEnumerator SpawnRoutine()
    {
        // Ждем перед первым спавном
        yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

        while (true) // Бесконечный цикл
        {
            // Удаляем из списка "пустые" ссылки (предметы, которые игрок уже подобрал/уничтожил)
            _spawnedItems.RemoveAll(item => item == null);

            // Если на карте меньше предметов, чем лимит — создаем новый
            if (_spawnedItems.Count < maxItemsOnMap)
            {
                SpawnItem();
            }

            // Ждем случайное время перед следующей попыткой
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnItem()
    {
        // Выбираем случайную точку из списка
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Создаем предмет
        GameObject newItem = Instantiate(itemPrefab, randomPoint.position, Quaternion.identity);

        // Настраиваем его (если это патроны)
        AmmoPickup pickupScript = newItem.GetComponent<AmmoPickup>();
        if (pickupScript != null)
        {
            pickupScript.ammoAmount = Random.Range(minAmmoInBox, maxAmmoInBox + 1);
        }

        // Запоминаем созданный предмет
        _spawnedItems.Add(newItem);
    }
}