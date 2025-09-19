using UnityEngine;

[CreateAssetMenu(menuName = "Config/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Player")]
    public float baseMoveSpeed = 5f;
    public int baseHealth = 100;
    public float invulnerabilityTime = 2.0f;

    [Header("Combat")]
    public float parryWindow = 0.5f;
    public float parryVulnerableTime = 0.1f; 

    [Header("Dodge")]
    public float dodgeSpeed = 6f;
    public float dodgeDuration = 0.5f;
    public float dodgeVulnerableTime;
    public float dodgeColdown;

    [Header("Waves")]
    public int startWave = 1;
    public float timeBetweenWaves = 5f;
}