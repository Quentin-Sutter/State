using UnityEngine;

[CreateAssetMenu(fileName = "SO_Strike", menuName = "Scriptable Objects/SO_Strike")]
public class SO_Strike : ScriptableObject
{
    public string strikeName;
    public float delayStart;
    public float delayEnd;
    public float duration;
    public StrikeMovementType type;
    public float moveAmount;
}

public enum StrikeMovementType
{
    Thrust,    // mouvement en avant
    Swing,     // Arc de cercle gauche - droite
    Spin,       // Tour complet
    BackSwing,     // Arc de cercle arrière  droite - gauche

}
