using UnityEngine;

public class EnemyAttackState : IEnemyState
{ 
    System.Action onStrikeFinishedHandler;

    public void Enter(Enemy enemy)
    {
        enemy.weaponHandler.Attack(enemy, enemy.playerScript.transform.position); 
        onStrikeFinishedHandler = () => StrikeFinished(enemy);
        enemy.weaponHandler.OnStrikeFinished += onStrikeFinishedHandler;
        enemy.expressions.SetExpression(CharacterExpression.Expression.Angry);
    }

    public void Update(Enemy enemy)
    {
      
    }

    public void Exit(Enemy enemy) 
    {
        enemy.weaponHandler.OnStrikeFinished -= onStrikeFinishedHandler; 
    }

    void StrikeFinished(Enemy enemy)
    {
        if (enemy.playerScript.StateMachine.CurrentState is PlayerDeathState)
        {
            return;
        } 

        if (Vector3.Distance(enemy.transform.position, enemy.playerScript.transform.position) <= enemy.attackRange)
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
