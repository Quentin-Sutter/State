using UnityEngine;

public class PlayerStateMachine
{
    private PlayerState currentState;
    public PlayerState CurrentState => currentState;

    public void Initialize(PlayerState startState)
    {
        currentState = startState;
        currentState?.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }


}
