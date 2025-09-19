using UnityEngine;

public class PlayerDodgeState : PlayerState
{
    public Vector2 inputVector;
    float dashSpeed;
    float dashTime;
    float vulnerableTime;

    float timer = 0.0f;

    public PlayerDodgeState(Player player) : base(player)
    {
    }


    public override void Enter()
    {
        dashTime = GameBalance.Config.dodgeDuration.ApplyPercentChange(player.fullUpgrade.dodgeDuration);
        dashSpeed = GameBalance.Config.dodgeSpeed.ApplyPercentChange(player.fullUpgrade.dodgeSpeed);
        vulnerableTime = GameBalance.Config.dodgeVulnerableTime;
        inputVector = player.lastInputVector;
        player.gameObject.layer = 8;
        
    }

    public override void Update()
    {
        if (timer < dashTime)
        {
            timer += Time.deltaTime;
            if (IsInvulnerable()) player.expressions.SetExpression(CharacterExpression.Expression.Sunglass);
            else player.expressions.SetExpression(CharacterExpression.Expression.Surprised);

            if (timer >= dashTime)
            {
                player.StateMachine.ChangeState(new PlayerMovementState(player));
            }
            else player.UpdateMovement(inputVector, dashSpeed);
        }
    }

    public override void Exit()
    {
        StopPlayerMovement();
        player.dashCoolDown = GameBalance.Config.dodgeColdown;
        player.gameObject.layer = 6;
    }

    public override bool IsInvulnerable()
    {

        return timer > vulnerableTime && timer < dashTime - vulnerableTime;
    }
}
