using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [Header("Точки спавна")]
    public List<Transform> allSpawnPoints;
    public int activeSpawnersCount = 3; // Сколько точек активно в одну волну
    public bool randomizeSpawnersEveryWave = true;

    [Header("Прогрессия")]
    public float countMultiplier = 1.2f; // На сколько умножать кол-во врагов каждую волну
    public float timeBetweenWaves = 5f;
    public float spawnRate = 1f; // Задержка между появлением отдельных врагов

    [Header("Конфигурация врагов")]
    public List<EnemyWaveConfig> enemyConfigs;

    private int _currentWaveIndex = 0;
    private int _enemiesAlive = 0;
    private bool _isSpawning = false;
    private bool _waitingForNextWave = false;
    private List<Transform> _currentWaveSpawners;
    private HUD _hud;

    void Start()
    {
        _hud = FindObjectOfType<HUD>();

        if (allSpawnPoints == null || allSpawnPoints.Count == 0)
        {
            enabled = false;
            return;
        }

        StartCoroutine(StartNextWave());
    }

    void OnEnable()
    {
        // Подписываемся на смерть любого врага
        EnemyHealth.OnEnemyDied += OnEnemyKilled;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= OnEnemyKilled;
    }

    private void OnEnemyKilled(float unusedFloat)
    {
        if (_enemiesAlive > 0) _enemiesAlive--;

        // Если враги кончились и спавн завершен -> конец волны
        if (_enemiesAlive <= 0 && !_isSpawning && !_waitingForNextWave)
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveCompleted(_currentWaveIndex);
        }
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        _waitingForNextWave = true;

        // Обратный отсчет перед следующей волной
        float timer = timeBetweenWaves;

        while (timer > 0)
        {
            if (_hud != null)
            {
                _hud.UpdateWaveText($"NEXT: {Mathf.CeilToInt(timer)}");
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        _waitingForNextWave = false;
        _currentWaveIndex++;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveStarted(_currentWaveIndex);
        }

        if (_hud != null)
        {
            _hud.UpdateWaveText($"WAVE {_currentWaveIndex}");
        }

        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        _isSpawning = true;
        SelectActiveSpawners();

        // Генерируем список врагов, которых нужно заспавнить в этой волне
        List<GameObject> spawnQueue = GenerateSpawnQueue();
        _enemiesAlive = spawnQueue.Count;

        foreach (GameObject enemyPrefab in spawnQueue)
        {
            Transform spawnPoint = GetRandomActiveSpawner();
            if (spawnPoint != null)
            {
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(spawnRate);
        }

        _isSpawning = false;
    }

    // Выбирает подмножество точек спавна
    private void SelectActiveSpawners()
    {
        if (!randomizeSpawnersEveryWave || activeSpawnersCount <= 0 || activeSpawnersCount >= allSpawnPoints.Count)
        {
            _currentWaveSpawners = new List<Transform>(allSpawnPoints);
        }
        else
        {
            // Перемешиваем список и берем первые N элементов (LINQ)
            _currentWaveSpawners = allSpawnPoints.OrderBy(x => Random.value).Take(activeSpawnersCount).ToList();
        }
    }

    // Рассчитывает кол-во врагов по формуле прогрессии
    private List<GameObject> GenerateSpawnQueue()
    {
        List<GameObject> queue = new List<GameObject>();
        float currentMultiplier = Mathf.Pow(countMultiplier, _currentWaveIndex - 1);

        foreach (var config in enemyConfigs)
        {
            int countToSpawn = Mathf.RoundToInt(config.baseCount * currentMultiplier);
            for (int i = 0; i < countToSpawn; i++) queue.Add(config.enemyPrefab);
        }

        // Перемешиваем очередь (алгоритм Фишера-Йейтса), чтобы враги разных типов шли вперемешку
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

[System.Serializable]
public class EnemyWaveConfig
{
    public string name = "Enemy Type";
    public GameObject enemyPrefab;
    public int baseCount = 5; // Базовое количество на 1-й волне
}