using UnityEngine;

public class PlayerMovementState : PlayerState
{
    private Vector2 inputVector;
    public float deadZone = 0.1f;

    public PlayerMovementState(Player player) : base(player)
    {
    } 

    public override void Enter()
    {
        player.Expressions.SetExpression(CharacterExpression.Expression.Happy);
    }

    public override void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 rawInput = new Vector2(horizontal, vertical);
        if (rawInput.magnitude < deadZone)
        {
            inputVector = Vector2.zero;
        }
        else
        {
            inputVector = rawInput;
        }

        player.UpdateMovement(inputVector);
    }

    public override void Exit()
    {
        StopPlayerMovement();
    }
}