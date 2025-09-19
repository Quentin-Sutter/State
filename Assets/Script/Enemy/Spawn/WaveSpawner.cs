using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaveSpawner : MonoBehaviour
{
    public EnemyFactory enemyFactory;
    public ScriptableObject waveStrategyObject;
    private IWaveStrategy waveStrategy;

    public Transform[] spawnPoints;
    public float waveMenuTime = 3f;

    public int maxWaves;
    private int currentWave = 1;

    int deadEnemies;
    int spawnedEnemies;

    Player player;

    public UnityEvent<int, int> OnWaveStart;
    public UnityEvent OnWaveFinished;
    public UnityEvent OnAllWavesFinished;

    public void Initialize ()
    {
        waveStrategy = waveStrategyObject as IWaveStrategy;
        player = FindFirstObjectByType<Player>();
        enemyFactory.enemyDeath.AddListener(EnemyDeath);
    }

    public IEnumerator SpawnWave()
    {
        var enemies = waveStrategy.GetWaveEnemies(currentWave);
        WaveStart(enemies.Count);

        yield return new WaitForSeconds(waveMenuTime); // wait for menu

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
            yield return new WaitForSeconds(0.3f); // délai entre spawns
        }

        yield return new WaitUntil(()=> deadEnemies == spawnedEnemies);
        OnWaveFinished?.Invoke();
        if (IsLastWave()) OnAllWavesFinished?.Invoke();
        else currentWave++;
    }

    void WaveStart (int enemiesNumber)
    {
        OnWaveStart?.Invoke(currentWave, maxWaves);
        deadEnemies = 0;
        spawnedEnemies = enemiesNumber;
    } 

    void EnemyDeath ()
    {
        deadEnemies++;
    }

    Vector3 GetRandomSpawnPoint ()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return spawnPoint.position;
    }

    public bool IsLastWave ()
    {
        return currentWave == maxWaves;
    }

    public bool IsBossWave ()
    {
        return IsLastWave() && waveStrategy.GetBossData() != null;
    }
}
