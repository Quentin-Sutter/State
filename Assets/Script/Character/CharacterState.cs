using UnityEngine;

public abstract class CharacterState : ICharacterState
{
    protected CharacterState(CharacterStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    protected CharacterStateMachine StateMachine { get; }
    protected CharacterContext Context => StateMachine.Context;
    protected Character Character => Context.Character;

    public virtual bool IsInvulnerable => false;
    public virtual bool IsCountering => false;

    public abstract void Enter(CharacterContext context);
    public abstract void Update(CharacterContext context);
    public abstract void Exit(CharacterContext context);

    protected void StopMovement()
    {
        Character?.StopMovement();
    }

    protected void SetExpression(CharacterExpression.Expression expression)
    {
        if (Character?.Expressions != null)
        {
            Character.Expressions.SetExpression(expression);
        }
    }
}
