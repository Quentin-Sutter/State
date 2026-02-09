using UnityEngine;

public class CharacterMovementState : CharacterState
{
    public CharacterMovementState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter(CharacterContext context)
    {
        SetExpression(CharacterExpression.Expression.Happy);
    }

    public override void Update(CharacterContext context)
    {
        var moveInput = context.Input.Move;
        if (moveInput.magnitude < context.MoveDeadZone)
        {
            moveInput = Vector2.zero;
        }

        Character.UpdateMovement(moveInput);

        if (context.Input.AttackPressed)
        {
            StateMachine.ChangeState(new CharacterAttackState(StateMachine));
            return;
        }

        if (context.Input.DodgePressed && context.DodgeCooldownRemaining <= 0f)
        {
            StateMachine.ChangeState(new CharacterDodgeState(StateMachine));
            return;
        }

        if (context.Input.ParryPressed)
        {
            StateMachine.ChangeState(new CharacterParryState(StateMachine));
        }
    }

    public override void Exit(CharacterContext context)
    {
        StopMovement();
    }
}
