using UnityEngine;

public class CharacterDeathState : CharacterState
{
    public CharacterDeathState(CharacterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter(CharacterContext context)
    {
        StopMovement();
        if (Character.Skin != null)
        {
            Character.Skin.color = Color.black;
        }
    }

    public override void Update(CharacterContext context)
    {
    }

    public override void Exit(CharacterContext context)
    {
    }
}
