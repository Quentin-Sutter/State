using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaveSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private ScriptableObject waveStrategyObject;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float delayBetweenSpawns = 0.3f;
    [SerializeField] private float waveMenuTime = 3f;
    [SerializeField] private int maxWaves = 1;

    private IWaveStrategy waveStrategy;
    private Player player;

    private int currentWave = 1;
    private int deadEnemies;
    private int spawnedEnemies;

    public UnityEvent<int, int> OnWaveStart;
    public UnityEvent OnWaveFinished;
    public UnityEvent OnAllWavesFinished;

    public void Initialize()
    {
        waveStrategy = waveStrategyObject as IWaveStrategy;
        if (waveStrategy == null)
        {
            Debug.LogError("Wave strategy scriptable object does not implement IWaveStrategy.");
            enabled = false;
            return;
        }

        player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("WaveSpawner could not find the Player in the scene.");
            enabled = false;
            return;
        }

        if (enemyFactory == null)
        {
            Debug.LogError("WaveSpawner is missing an EnemyFactory reference.");
            enabled = false;
            return;
        }

        enemyFactory.enemyDeath.AddListener(OnEnemyDeath);
    }

    private void OnDestroy()
    {
        if (enemyFactory != null)
        {
            enemyFactory.enemyDeath.RemoveListener(OnEnemyDeath);
        }
    }

    public IEnumerator SpawnWave()
    {
        if (waveStrategy == null)
        {
            yield break;
        }

        var enemies = waveStrategy.GetWaveEnemies(currentWave);
        StartWave();

        yield return new WaitForSeconds(waveMenuTime);

        if (currentWave == maxWaves)
        {
            var boss = waveStrategy.GetBossData();
            if (boss != null)
            {
                enemyFactory.SpawnEnemy(boss, GetRandomSpawnPoint(), player);
                spawnedEnemies++;
            }
        }

        foreach (var enemyData in enemies)
        {
            enemyFactory.SpawnEnemy(enemyData, GetRandomSpawnPoint(), player);
            spawnedEnemies++;
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        yield return new WaitUntil(() => deadEnemies >= spawnedEnemies);

        OnWaveFinished?.Invoke();

        if (IsLastWave())
        {
            OnAllWavesFinished?.Invoke();
        }
        else
        {
            currentWave++;
        }
    }

    public bool IsLastWave()
    {
        return currentWave >= maxWaves;
    }

    public bool IsBossWave()
    {
        return IsLastWave() && waveStrategy?.GetBossData() != null;
    }

    private void StartWave()
    {
        OnWaveStart?.Invoke(currentWave, maxWaves);
        deadEnemies = 0;
        spawnedEnemies = 0;
    }

    private void OnEnemyDeath()
    {
        deadEnemies++;
    }

    private Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return transform.position;
        }

        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return spawnPoint.position;
    }
}
