using UnityEngine;

public class CharacterParryState : CharacterState
{
    private float timer;
    private float parryTime;
    private float vulnerableTime;

    public CharacterParryState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter(CharacterContext context)
    {
        StopMovement();
        timer = 0f;
        parryTime = GameBalance.Config.parryWindow.ApplyPercentChange(Character.FullUpgrade.parryDuration);
        vulnerableTime = GameBalance.Config.parryVulnerableTime;
    }

    public override void Update(CharacterContext context)
    {
        if (timer < parryTime)
        {
            timer += Time.deltaTime;
            SetExpression(IsInvulnerable ? CharacterExpression.Expression.Sunglass : CharacterExpression.Expression.Surprised);

            if (timer >= parryTime)
            {
                StateMachine.ChangeState(new CharacterMovementState(StateMachine));
            }
        }
    }

    public override void Exit(CharacterContext context)
    {
    }

    public override bool IsInvulnerable
    {
        get
        {
            return timer > vulnerableTime && timer < parryTime - vulnerableTime;
        }
    }
}
