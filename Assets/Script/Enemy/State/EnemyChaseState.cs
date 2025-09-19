using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        //Debug.Log("Enemy is now Chasing");
        enemy.expressions.SetExpression(CharacterExpression.Expression.Surprised);
    }

    public void Update(Enemy enemy)
    {
        if (enemy.playerScript.StateMachine.CurrentState is PlayerDeathState)
        {
            enemy.ChangeState(new EnemyIdleState());
        }

        Vector2 direction = enemy.playerScript.transform.position - enemy.transform.position;

        float distance = direction.magnitude;

        enemy.UpdateMovement(direction.normalized);

        if (distance < enemy.attackRange)
        {
            enemy.ChangeState(new EnemyAttackState());
        }
        else if (distance > enemy.chaseRange)
        {
            enemy.ChangeState(new EnemyIdleState());
        }
    }

    public void Exit(Enemy enemy) 
    {
        enemy.StopMovement();
    }
}
