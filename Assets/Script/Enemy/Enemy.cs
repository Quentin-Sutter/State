using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1.5f;

    private IEnemyState currentState;

    public Player Target { get; private set; }
    public float ChaseRange => chaseRange;
    public float AttackRange => attackRange;

    public void Initialize(EnemyData data, Player player, EnemyFactory factory)
    {
        Target = player;

        Health.Initialize(data.baseHealth);
        WeaponHandler.CurrentWeapon = data.weapon;

        attackRange = data.attackRange;
        chaseRange = data.chaseRange;

        SetUpgrades(data.upgrades);

        ChangeState(new EnemyIdleState());
        Health.OnDeath.AddListener(Death);
        Health.OnDeath.AddListener(() => factory.enemyDeath.Invoke());

        base.Init();
    }

    private void Update()
    {
        currentState?.Update(this);
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public override void TakeDamage(Character origin)
    {
        base.TakeDamage(origin);
        ChangeState(new EnemyDamageState(this, GetPushValues(this, origin)));
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}
