public interface ICharacterState
{
    void Enter(CharacterContext context);
    void Update(CharacterContext context);
    void Exit(CharacterContext context);
}
