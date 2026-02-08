using System;
using UnityEngine;

public class CharacterAttackState : CharacterState
{
    private const float ComboInputDelay = 0.2f;

    private bool fromParry;
    private bool continueCombo;
    private float comboInputTimer;
    private Action onStrikeFinishedHandler;

    public CharacterAttackState(CharacterStateMachine stateMachine, bool fromParry = false) : base(stateMachine)
    {
        this.fromParry = fromParry;
    }

    public override void Enter(CharacterContext context)
    {
        continueCombo = false;
        comboInputTimer = ComboInputDelay;
        StopMovement();

        var targetPoint = ResolveTargetPoint(context);
        Character.WeaponHandler.Attack(Character, targetPoint);

        onStrikeFinishedHandler = HandleStrikeFinished;
        Character.WeaponHandler.OnStrikeFinished += onStrikeFinishedHandler;
        SetExpression(CharacterExpression.Expression.Angry);
    }

    public override void Update(CharacterContext context)
    {
        if (comboInputTimer > 0f)
        {
            comboInputTimer -= Time.deltaTime;
            return;
        }

        if (context.Input.AttackPressed)
        {
            continueCombo = true;
        }
    }

    public override void Exit(CharacterContext context)
    {
        if (onStrikeFinishedHandler != null)
        {
            Character.WeaponHandler.OnStrikeFinished -= onStrikeFinishedHandler;
        }

        if (!continueCombo)
        {
            Character.ComboFinished();
        }
    }

    public override bool IsCountering => fromParry;
    public override bool IsInvulnerable => fromParry;

    public void StopCountering()
    {
        fromParry = false;
    }

    private void HandleStrikeFinished()
    {
        if (continueCombo)
        {
            StateMachine.ChangeState(new CharacterAttackState(StateMachine, fromParry));
        }
        else
        {
            StateMachine.ChangeState(new CharacterMovementState(StateMachine));
        }

        continueCombo = false;
    }

    private Vector3 ResolveTargetPoint(CharacterContext context)
    {
        if (context.HasLastAimPosition)
        {
            return context.LastAimPosition;
        }

        var direction = context.LastMoveInput;
        if (direction.sqrMagnitude <= 0.001f)
        {
            direction = Vector2.right;
        }

        return (Vector2)Character.transform.position + direction;
    }
}
