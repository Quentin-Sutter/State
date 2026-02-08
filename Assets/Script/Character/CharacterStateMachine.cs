public class CharacterStateMachine
{
    private ICharacterState currentState;

    public CharacterStateMachine(CharacterContext context)
    {
        Context = context;
    }

    public CharacterContext Context { get; }
    public ICharacterState CurrentState => currentState;

    public void Initialize(ICharacterState startState)
    {
        currentState = startState;
        currentState?.Enter(Context);
    }

    public void ChangeState(ICharacterState newState)
    {
        currentState?.Exit(Context);
        currentState = newState;
        currentState?.Enter(Context);
    }

    public void Update()
    {
        Context.UpdateInput();
        currentState?.Update(Context);
    }
}
