using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public int baseHealth;
    public float moveSpeed = 3.0f;
    public float chaseRange = 5f;
    public float attackRange = 1.5f; 
    public SO_Weapon weapon;
    public SO_Upgrade[] upgrades;
}
