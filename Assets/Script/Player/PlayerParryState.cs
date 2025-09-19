using UnityEngine;

public class PlayerParryState : PlayerState
{
    float timer = 0.0f;
    float parryTime;
    float vulnerableTime;

    public PlayerParryState(Player player) : base(player)
    {
    }

   

    public override void Enter()
    {
        player.StopMovement();
        parryTime = GameBalance.Config.parryWindow.ApplyPercentChange(player.fullUpgrade.parryDuration);
        vulnerableTime = GameBalance.Config.parryVulnerableTime;
    }

    public override void Update()
    {
        if (timer < parryTime)
        {
            timer += Time.deltaTime;
            if (IsInvulnerable()) player.expressions.SetExpression(CharacterExpression.Expression.Sunglass);
            else player.expressions.SetExpression(CharacterExpression.Expression.Surprised);

            if (timer >= parryTime)
            {
                player.StateMachine.ChangeState(new PlayerMovementState(player));
            }
        }
    }

    public override void Exit()
    { 

    }

    public override bool IsInvulnerable()
    {
        return timer > vulnerableTime && timer < parryTime - vulnerableTime;
    }
}
