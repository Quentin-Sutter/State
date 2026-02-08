using UnityEngine;

public class EnemyDecisionMaker : MonoBehaviour, ICharacterInputSource
{
    [SerializeField] private Character target;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float chaseRange = 5f;

    public CharacterInputData GetInput()
    {
        var input = new CharacterInputData();

        if (target == null)
        {
            return input;
        }

        var direction = target.transform.position - transform.position;
        var distance = direction.magnitude;

        if (distance <= chaseRange)
        {
            input.Move = direction.normalized;
        }

        if (distance <= attackRange)
        {
            input.AttackPressed = true;
        }

        input.AimPosition = target.transform.position;
        input.HasAimPosition = true;

        return input;
    }
}
