using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleWavesStrategy", menuName = "Scriptable Objects/SimpleWavesStrategy")]
public class SimpleWavesStrategy : ScriptableObject, IWaveStrategy
{
    public EnemyData[] possibleEnemies;
    public EnemyData Boss;

    public EnemyData GetBossData()
    { 
        return Boss;
    }

    public List<EnemyData> GetWaveEnemies(int waveNumber)
    {
        var enemies = new List<EnemyData>();
        int enemyCount = Mathf.Min(waveNumber * 2, 20); // limite à 20 max

        for (int i = 0; i < enemyCount; i++)
        {
            enemies.Add(possibleEnemies[Random.Range(0, possibleEnemies.Length)]);
        }
        return enemies;
    }

    
}
