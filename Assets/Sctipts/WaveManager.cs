using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Нужен для удобной работы со списками

public class WaveManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Все точки на карте, где могут появляться враги")]
    public List<Transform> allSpawnPoints;

    [Tooltip("Сколько спавнеров использовать одновременно в одной волне? (0 = все сразу)")]
    public int activeSpawnersCount = 3;
    public bool randomizeSpawnersEveryWave = true;

    [Header("Wave Progression")]
    [Tooltip("Множитель количества врагов для каждой следующей волны (например, 1.2 = +20%)")]
    public float countMultiplier = 1.2f;
    [Tooltip("Время отдыха между волнами (сек)")]
    public float timeBetweenWaves = 5f;
    [Tooltip("Задержка между появлением врагов внутри волны (сек)")]
    public float spawnRate = 1f;

    [Header("Enemies Configuration")]
    public List<EnemyWaveConfig> enemyConfigs;

    // Состояние системы
    private int _currentWaveIndex = 0;
    private int _enemiesAlive = 0;
    private bool _isSpawning = false;
    private bool _waitingForNextWave = false;

    // Список активных точек для текущей волны
    private List<Transform> _currentWaveSpawners;

    void Start()
    {
        if (allSpawnPoints == null || allSpawnPoints.Count == 0)
        {
            Debug.LogError("WaveManager: Не назначены точки спавна (Spawn Points)!");
            enabled = false;
            return;
        }

        // Запускаем первую волну
        StartCoroutine(StartNextWave());
    }

    void OnEnable()
    {
        // Подписываемся на событие смерти из твоего EnemyHealth
        EnemyHealth.OnEnemyDied += OnEnemyKilled;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= OnEnemyKilled;
    }

    private void OnEnemyKilled(float unusedFloat)
    {
        // Уменьшаем счетчик живых врагов
        if (_enemiesAlive > 0)
        {
            _enemiesAlive--;
        }

        // Если враги кончились и мы не спавним прямо сейчас -> готовим следующую волну
        if (_enemiesAlive <= 0 && !_isSpawning && !_waitingForNextWave)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {
        _waitingForNextWave = true;
        _currentWaveIndex++;

        Debug.Log($"--- ВОЛНА {_currentWaveIndex} НАЧНЕТСЯ ЧЕРЕЗ {timeBetweenWaves} СЕК ---");

        // Ждем перерыва перед волной
        yield return new WaitForSeconds(timeBetweenWaves);

        _waitingForNextWave = false;
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        _isSpawning = true;

        // 1. Выбираем спавнеры для этой волны
        SelectActiveSpawners();

        // 2. Генерируем список всех врагов на эту волну (очередь спавна)
        List<GameObject> spawnQueue = GenerateSpawnQueue();

        // Устанавливаем количество живых врагов, которых игроку нужно победить
        _enemiesAlive = spawnQueue.Count;

        Debug.Log($"Волна {_currentWaveIndex}: Врагов {_enemiesAlive}. Активных точек: {_currentWaveSpawners.Count}");

        // 3. Спавним врагов из очереди
        foreach (GameObject enemyPrefab in spawnQueue)
        {
            // Выбираем случайную точку из АКТИВНЫХ спавнеров
            Transform spawnPoint = GetRandomActiveSpawner();

            if (spawnPoint != null)
            {
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            }

            // Ждем перед следующим спавном
            yield return new WaitForSeconds(spawnRate);
        }

        _isSpawning = false;
    }

    // Логика выбора спавнеров
    private void SelectActiveSpawners()
    {
        // Если настроено использовать все или число больше чем есть точек -> берем все
        if (!randomizeSpawnersEveryWave || activeSpawnersCount <= 0 || activeSpawnersCount >= allSpawnPoints.Count)
        {
            _currentWaveSpawners = new List<Transform>(allSpawnPoints);
        }
        else
        {
            // Перемешиваем список всех точек и берем первые N штук
            // Используем System.Random или Unity Random для перемешивания
            _currentWaveSpawners = allSpawnPoints.OrderBy(x => Random.value).Take(activeSpawnersCount).ToList();
        }
    }

    // Генерация списка врагов с учетом автолевелинга
    private List<GameObject> GenerateSpawnQueue()
    {
        List<GameObject> queue = new List<GameObject>();

        // Рассчитываем текущий множитель: Multiplier ^ (Wave - 1)
        // Пример: 1.2 ^ 0 = 1; 1.2 ^ 1 = 1.2; 1.2 ^ 2 = 1.44...
        float currentMultiplier = Mathf.Pow(countMultiplier, _currentWaveIndex - 1);

        foreach (var config in enemyConfigs)
        {
            // Базовое кол-во * множитель
            int countToSpawn = Mathf.RoundToInt(config.baseCount * currentMultiplier);

            for (int i = 0; i < countToSpawn; i++)
            {
                queue.Add(config.enemyPrefab);
            }
        }

        // Перемешиваем очередь, чтобы враги шли вразнобой (алгоритм Фишера-Йейтса упрощенный)
        for (int i = 0; i < queue.Count; i++)
        {
            GameObject temp = queue[i];
            int randomIndex = Random.Range(i, queue.Count);
            queue[i] = queue[randomIndex];
            queue[randomIndex] = temp;
        }

        return queue;
    }

    private Transform GetRandomActiveSpawner()
    {
        if (_currentWaveSpawners.Count == 0) return null;
        return _currentWaveSpawners[Random.Range(0, _currentWaveSpawners.Count)];
    }
}

// Вспомогательный класс для настроек в инспекторе
[System.Serializable]
public class EnemyWaveConfig
{
    public string name = "Enemy Type"; // Просто для удобства в инспекторе
    public GameObject enemyPrefab;
    public int baseCount = 5; // Сколько штук спавнить на 1-й волне
}