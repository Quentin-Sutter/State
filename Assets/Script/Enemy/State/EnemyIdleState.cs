using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        //Debug.Log("Enemy is now Idle");
        enemy.Expressions.SetExpression(CharacterExpression.Expression.Sleep);
    }

    public void Update(Enemy enemy)
    {
        if (enemy.Target.StateMachine.CurrentState is PlayerDeathState)
        {
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.Target.transform.position);

        if (distance < enemy.ChaseRange)
        {
            enemy.ChangeState(new EnemyChaseState());
        }
    }

    public void Exit(Enemy enemy) { }
}
