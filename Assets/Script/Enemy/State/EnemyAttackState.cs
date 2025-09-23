using System;
using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private Action onStrikeFinishedHandler;

    public void Enter(Enemy enemy)
    {
        enemy.WeaponHandler.Attack(enemy, enemy.Target.transform.position);
        onStrikeFinishedHandler = () => StrikeFinished(enemy);
        enemy.WeaponHandler.OnStrikeFinished += onStrikeFinishedHandler;
        enemy.Expressions.SetExpression(CharacterExpression.Expression.Angry);
    }

    public void Update(Enemy enemy)
    {

    }

    public void Exit(Enemy enemy)
    {
        enemy.WeaponHandler.OnStrikeFinished -= onStrikeFinishedHandler;
    }

    void StrikeFinished(Enemy enemy)
    {
        if (enemy.Target.StateMachine.CurrentState is PlayerDeathState)
        {
            return;
        }

        if (Vector3.Distance(enemy.transform.position, enemy.Target.transform.position) <= enemy.AttackRange)
        {
            enemy.ChangeState(new EnemyAttackState());
        }
        else
        {
            enemy.ComboFinished();
            enemy.ChangeState(new EnemyChaseState());
        }
    }
}
