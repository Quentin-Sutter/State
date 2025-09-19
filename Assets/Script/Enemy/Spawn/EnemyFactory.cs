using UnityEngine;
using UnityEngine.Events;

public class EnemyFactory : MonoBehaviour
{
    public UnityEvent enemyDeath = new UnityEvent();

    public GameObject SpawnEnemy(EnemyData data, Vector3 position, Player player)
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("EnemyData is not configured correctly.");
            return null;
        }

        var enemyObject = Instantiate(data.prefab, position, Quaternion.identity);
        if (!enemyObject.TryGetComponent<Enemy>(out var enemy))
        {
            Debug.LogError($"Spawned enemy prefab {data.prefab.name} has no Enemy component.");
            Destroy(enemyObject);
            return null;
        }

        enemy.Initialize(data, player, this);
        return enemyObject;
    }
}
