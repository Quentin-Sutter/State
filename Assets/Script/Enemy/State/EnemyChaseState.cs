using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        //Debug.Log("Enemy is now Chasing");
        enemy.Expressions.SetExpression(CharacterExpression.Expression.Surprised);
    }

    public void Update(Enemy enemy)
    {
        if (enemy.Target.StateMachine.CurrentState is PlayerDeathState)
        {
            enemy.ChangeState(new EnemyIdleState());
        }

        Vector2 direction = enemy.Target.transform.position - enemy.transform.position;

        float distance = direction.magnitude;

        enemy.UpdateMovement(direction.normalized);

        if (distance < enemy.AttackRange)
        {
            enemy.ChangeState(new EnemyAttackState());
        }
        else if (distance > enemy.ChaseRange)
        {
            enemy.ChangeState(new EnemyIdleState());
        }
    }

    public void Exit(Enemy enemy) 
    {
        enemy.StopMovement();
    }
}
