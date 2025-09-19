using System.Collections.Generic;
using UnityEngine;

public interface IWaveStrategy
{
    List<EnemyData> GetWaveEnemies(int waveNumber);

    EnemyData GetBossData();
}
