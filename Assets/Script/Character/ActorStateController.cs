using UnityEngine;

public class ActorStateController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Character character;
    [SerializeField] private MonoBehaviour inputSource;
    [SerializeField] private Character target;

    [Header("Setup")]
    [SerializeField] private bool isPlayer;
    [SerializeField] private float moveDeadZone = 0.1f;

    private bool initialized;

    public CharacterStateMachine StateMachine { get; private set; }
    public CharacterContext Context { get; private set; }

    private ICharacterInputSource InputSource => inputSource as ICharacterInputSource;

    private void Awake()
    {
        if (character == null)
        {
            character = GetComponent<Character>();
        }

        if (inputSource == null)
        {
            inputSource = GetComponent<ICharacterInputSource>() as MonoBehaviour;
        }
    }

    private void OnEnable()
    {
        InitializeStateMachine();
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        TickCooldowns();
        StateMachine.Update();
    }

    public void SetTarget(Character newTarget)
    {
        target = newTarget;
        Context?.SetTarget(newTarget);
    }

    private void InitializeStateMachine()
    {
        if (initialized)
        {
            return;
        }

        if (character == null)
        {
            Debug.LogError("ActorStateController requires a Character reference.");
            return;
        }

        var input = InputSource;
        if (input == null && inputSource != null)
        {
            Debug.LogError($"{name} input source does not implement ICharacterInputSource.");
        }

        Context = isPlayer
            ? new PlayerCharacterContext(character, input)
            : new EnemyCharacterContext(character, input, target);

        Context.MoveDeadZone = moveDeadZone;

        StateMachine = new CharacterStateMachine(Context);
        StateMachine.Initialize(new CharacterMovementState(StateMachine));
        initialized = true;
    }

    private void TickCooldowns()
    {
        if (Context.DodgeCooldownRemaining > 0f)
        {
            Context.DodgeCooldownRemaining -= Time.deltaTime;
        }
    }

    private void SubscribeEvents()
    {
        if (character == null)
        {
            return;
        }

        character.Damaged += HandleDamaged;
        if (character.Health != null)
        {
            character.Health.OnDeath.AddListener(HandleDeath);
        }
    }

    private void UnsubscribeEvents()
    {
        if (character == null)
        {
            return;
        }

        character.Damaged -= HandleDamaged;
        if (character.Health != null)
        {
            character.Health.OnDeath.RemoveListener(HandleDeath);
        }
    }

    private bool HandleDamaged(Character origin)
    {
        if (!initialized || origin == null)
        {
            return true;
        }

        if (StateMachine.CurrentState is CharacterState state && state.IsInvulnerable)
        {
            if (state is CharacterParryState)
            {
                Context.LastAimPosition = origin.transform.position;
                Context.HasLastAimPosition = true;
                StateMachine.ChangeState(new CharacterAttackState(StateMachine, true));
            }

            return false;
        }

        var pushValues = character.GetPushValues(character, origin);
        StateMachine.ChangeState(new CharacterDamageState(StateMachine, pushValues));
        return true;
    }

    private void HandleDeath()
    {
        if (!initialized)
        {
            return;
        }

        StateMachine.ChangeState(new CharacterDeathState(StateMachine));
    }
}
