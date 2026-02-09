using UnityEngine;

public class CharacterDodgeState : CharacterState
{
    private Vector2 inputVector;
    private float dashSpeed;
    private float dashTime;
    private float vulnerableTime;
    private float timer;

    public CharacterDodgeState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter(CharacterContext context)
    {
        dashTime = GameBalance.Config.dodgeDuration.ApplyPercentChange(Character.FullUpgrade.dodgeDuration);
        dashSpeed = GameBalance.Config.dodgeSpeed.ApplyPercentChange(Character.FullUpgrade.dodgeSpeed);
        vulnerableTime = GameBalance.Config.dodgeVulnerableTime;

        inputVector = context.LastMoveInput;
        if (inputVector.sqrMagnitude <= 0.001f)
        {
            inputVector = Vector2.right;
        }

        timer = 0f;
    }

    public override void Update(CharacterContext context)
    {
        if (timer < dashTime)
        {
            timer += Time.deltaTime;
            SetExpression(IsInvulnerable ? CharacterExpression.Expression.Sunglass : CharacterExpression.Expression.Surprised);

            if (timer >= dashTime)
            {
                StateMachine.ChangeState(new CharacterMovementState(StateMachine));
            }
            else
            {
                Character.UpdateMovement(inputVector, dashSpeed);
            }
        }
    }

    public override void Exit(CharacterContext context)
    {
        StopMovement();
        context.DodgeCooldownRemaining = GameBalance.Config.dodgeCooldown;
    }

    public override bool IsInvulnerable
    {
        get
        {
            return timer > vulnerableTime && timer < dashTime - vulnerableTime;
        }
    }
}
