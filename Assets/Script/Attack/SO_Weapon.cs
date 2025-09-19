using UnityEngine;

[CreateAssetMenu(fileName = "SO_Weapon", menuName = "Scriptable Objects/SO_Weapon")]
public class SO_Weapon : ScriptableObject
{
    public string weaponName;
    public Weapon shape;
    public int damage;

    public SO_Strike[] comboStrikes; 

    public float pushPower = 10.0f;
    public float pushDuration = 0.5f;

    public SO_Upgrade[] upgradesOnStart;
}
