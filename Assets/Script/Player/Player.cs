using DG.Tweening;
using UnityEngine;

public class Player : Character
{ 
    public bool canControl;
    public float dashCoolDown;
    public PlayerStateMachine StateMachine { get; private set; }

    [HideInInspector] public Vector2 lastPosClick;
    [HideInInspector] public Vector2 lastInputVector;

    public SO_Upgrade parryUpgrade;

    public float afterHitInvulnerability;
    public SpriteRenderer shield;

    public Vector2 GetVelocity()
    {
        return rb.linearVelocity;
    } 

    public void Initialialize ()
    {
        health.Initialize(GameBalance.Config.baseHealth); 
        RemoveControlPlayer();
        StateMachine = new PlayerStateMachine();
        StateMachine.Initialize(new PlayerMovementState(this)); 
        health.OnDeath.AddListener(() => Death());
        base.Init();
    }

    private void Update()
    {
        if (canControl)
        {
            HandleInput();
        }

        StateMachine.Update();

        if (afterHitInvulnerability > 0.0f) afterHitInvulnerability -= Time.deltaTime;

        shield.gameObject.SetActive(PlayerInvulnerable());

        if (dashCoolDown > 0.0f) dashCoolDown -= Time.deltaTime;
    }

    void HandleInput()
    {
        if (StateMachine.CurrentState is PlayerMovementState)
        { 
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RegisterClick();
                StateMachine.ChangeState(new PlayerAttackState(this));
            }

            if (Input.GetKeyDown(KeyCode.Space) && dashCoolDown <= 0.0f)
            {
                StateMachine.ChangeState(new PlayerDodgeState(this)); 
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StateMachine.ChangeState(new PlayerParryState(this));
            }
        } 
    }

    public override void UpdateMovement(Vector2 direction, float speed = 0.0f)
    {
        base.UpdateMovement(direction, speed);
        lastInputVector = direction;
    } 

    public void RegisterClick ()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        lastPosClick = mouseWorldPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(this);
        }
    }

    public override void TakeDamage(Character origin)
    {
        bool isHurt = !PlayerInvulnerable(); 

        if (StateMachine.CurrentState is PlayerParryState) 
        {
            if (PlayerInvulnerable())
            {
                lastPosClick = origin.transform.position;
                AddParryModifier();
                StateMachine.ChangeState(new PlayerAttackState(this, true));
            }
        }

        if (isHurt)
        {
            StateMachine.ChangeState(new PlayerDamageState(this, GetPushValues(this, origin)));
            base.TakeDamage(origin);
            afterHitInvulnerability = GameBalance.Config.invulnerabilityTime.ApplyPercentChange(fullUpgrade.invulnerableTime);
        }
    }

 

    public void AddUpgrade (SO_Upgrade upgrade)
    {
        upgrades.Add(upgrade);
        CreateFullUpgrade();
    }

    public void RemoveUpgrade (SO_Upgrade upgrade)
    {
        upgrades.Remove(upgrade);
        CreateFullUpgrade();
    }

    public void AddParryModifier ()
    {
        AddUpgrade(parryUpgrade); 
    }

    public void RemoveParryModifier ()
    {
        RemoveUpgrade(parryUpgrade);
    }

    void Death ()
    {
        StateMachine.ChangeState(new PlayerDeathState(this));
    }

    public void RemoveControlPlayer ()
    {
        canControl = false;
    }

    public void GiveControlPlayer ()
    {
        canControl = true;
    }

    public bool PlayerInvulnerable()
    {
        return StateMachine.CurrentState.IsInvulnerable() || afterHitInvulnerability > 0.0f;
    } 

    public override void ComboFinished ()
    {
        base.ComboFinished();
        if (StateMachine.CurrentState.IsCountering())
        {
            RemoveParryModifier();
            if (StateMachine.CurrentState is PlayerAttackState)
            {
                ((PlayerAttackState)StateMachine.CurrentState).StopCountering();
            }
        }
    } 
}
