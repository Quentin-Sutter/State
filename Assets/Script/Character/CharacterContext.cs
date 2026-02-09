using UnityEngine;

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
    public Vector2 LastMoveInput { get; private set; }
    public Vector2 LastAimPosition { get; set; }
    public bool HasLastAimPosition { get; set; }
    public float MoveDeadZone { get; set; } = 0.1f;
    public float DodgeCooldownRemaining { get; set; }

    public virtual bool IsPlayer => false;
    public virtual Character Target { get; protected set; }

    public void UpdateInput()
    {
        Input = InputSource != null ? InputSource.GetInput() : default;

        if (Input.Move.sqrMagnitude > 0.001f)
        {
            LastMoveInput = Input.Move;
        }

        if (Input.HasAimPosition)
        {
            LastAimPosition = Input.AimPosition;
            HasLastAimPosition = true;
        }
    }

    public virtual void SetTarget(Character newTarget)
    {
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

    public override Character Target { get; protected set; }

    public override void SetTarget(Character newTarget)
    {
        Target = newTarget;
    }
}
