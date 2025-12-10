using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<Transform> allSpawnPoints;
    public int activeSpawnersCount = 3;
    public bool randomizeSpawnersEveryWave = true;

    [Header("Wave Progression")]
    public float countMultiplier = 1.2f;
    public float timeBetweenWaves = 5f;
    public float spawnRate = 1f;

    [Header("Enemies Configuration")]
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
        EnemyHealth.OnEnemyDied += OnEnemyKilled;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= OnEnemyKilled;
    }

    private void OnEnemyKilled(float unusedFloat)
    {
        if (_enemiesAlive > 0) _enemiesAlive--;

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

        // --- НОВАЯ ЛОГИКА ТАЙМЕРА ОБРАТНОГО ОТСЧЕТА ---
        float timer = timeBetweenWaves;

        while (timer > 0)
        {
            // Обновляем текст в HUD (например: "NEXT WAVE: 3...")
            if (_hud != null)
            {
                _hud.UpdateWaveText($"NEXT: {Mathf.CeilToInt(timer)}");
            }

            timer -= Time.deltaTime;
            yield return null; // Ждем следующий кадр
        }
        // -----------------------------------------------

        _waitingForNextWave = false;
        _currentWaveIndex++;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveStarted(_currentWaveIndex);
        }

        // Возвращаем нормальный текст волны
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

    private void SelectActiveSpawners()
    {
        if (!randomizeSpawnersEveryWave || activeSpawnersCount <= 0 || activeSpawnersCount >= allSpawnPoints.Count)
        {
            _currentWaveSpawners = new List<Transform>(allSpawnPoints);
        }
        else
        {
            _currentWaveSpawners = allSpawnPoints.OrderBy(x => Random.value).Take(activeSpawnersCount).ToList();
        }
    }

    private List<GameObject> GenerateSpawnQueue()
    {
        List<GameObject> queue = new List<GameObject>();
        float currentMultiplier = Mathf.Pow(countMultiplier, _currentWaveIndex - 1);

        foreach (var config in enemyConfigs)
        {
            int countToSpawn = Mathf.RoundToInt(config.baseCount * currentMultiplier);
            for (int i = 0; i < countToSpawn; i++) queue.Add(config.enemyPrefab);
        }

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
    public int baseCount = 5;
}   