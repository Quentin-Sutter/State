using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Enemy : Character
{
    public Player playerScript; 

    public float chaseRange = 5f;
    public float attackRange = 1.5f; 

    private IEnemyState currentState;

    public void Initialize(EnemyData data, Player player, EnemyFactory factory)
    {
        playerScript = player;
        health.Initialize(data.baseHealth);
        weaponHandler.currentWeapon = data.weapon;
        attackRange = data.attackRange;
        chaseRange = data.chaseRange;
        upgrades = data.upgrades.ToList();
        ChangeState(new EnemyIdleState());
        health.OnDeath.AddListener(()=> Death());
        health.OnDeath.AddListener(()=> factory.enemyDeath.Invoke()); 
        base.Init();
    }

    private void Update()
    {
        currentState.Update(this);
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;
        currentState.Enter(this);
    }

    public override void TakeDamage(Character origin)
    {
        base.TakeDamage(origin);
        ChangeState(new EnemyDamageState(this, GetPushValues(this, origin)));
    }

    void Death ()
    {
        Destroy(gameObject);
    }
}
