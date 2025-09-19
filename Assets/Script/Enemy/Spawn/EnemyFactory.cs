using UnityEngine;
using UnityEngine.Events;

public class EnemyFactory : MonoBehaviour
{
    public UnityEvent enemyDeath;

    public GameObject SpawnEnemy(EnemyData data, Vector3 position, Player player)
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("EnemyData invalide !");
            return null;
        }

        GameObject enemy = Instantiate(data.prefab, position, Quaternion.identity);

        enemy.GetComponent<Enemy>().Initialize(data, player, this);
        return enemy;
    }
}
