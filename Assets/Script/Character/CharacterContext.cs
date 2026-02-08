public class CharacterContext
{
    public CharacterContext(Character character, ICharacterInputSource inputSource)
    {
        Character = character;
        InputSource = inputSource;
    }

    public Character Character { get; }
    public ICharacterInputSource InputSource { get; }
    public CharacterInputData Input { get; private set; }

    public virtual bool IsPlayer => false;
    public virtual Character Target => null;

    public void UpdateInput()
    {
        Input = InputSource != null ? InputSource.GetInput() : default;
    }
}

public class PlayerCharacterContext : CharacterContext
{
    public PlayerCharacterContext(Character character, ICharacterInputSource inputSource)
        : base(character, inputSource)
    {
    }

    public override bool IsPlayer => true;
}

public class EnemyCharacterContext : CharacterContext
{
    public EnemyCharacterContext(Character character, ICharacterInputSource inputSource, Character target)
        : base(character, inputSource)
    {
        Target = target;
    }

    public override Character Target { get; }
}
