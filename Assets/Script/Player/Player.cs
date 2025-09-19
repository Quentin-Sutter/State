using UnityEngine;
using UnityEngine.Serialization;

public class Player : Character
{
    [FormerlySerializedAs("canControl")]
    [SerializeField] private bool canControl;

    [FormerlySerializedAs("dashCoolDown")]
    [SerializeField] private float dashCooldown;

    [HideInInspector] public Vector2 lastPosClick;
    [HideInInspector] public Vector2 lastInputVector;

    [SerializeField] private SO_Upgrade parryUpgrade;
    [SerializeField] private SpriteRenderer shield;

    private float postHitInvulnerability;

    public PlayerStateMachine StateMachine { get; private set; }

    public bool CanControl => canControl;

    public float DashCooldown
    {
        get => dashCooldown;
        set => dashCooldown = value;
    }

    public Vector2 GetVelocity()
    {
        return Rigidbody != null ? Rigidbody.linearVelocity : Vector2.zero;
    }

    public void Initialize()
    {
        Health.Initialize(GameBalance.Config.baseHealth);
        DisableControl();

        StateMachine = new PlayerStateMachine();
        StateMachine.Initialize(new PlayerMovementState(this));

        Health.OnDeath.AddListener(Death);
        base.Init();
    }

    private void Update()
    {
        if (CanControl)
        {
            HandleInput();
        }

        StateMachine?.Update();

        if (postHitInvulnerability > 0f)
        {
            postHitInvulnerability -= Time.deltaTime;
        }

        if (shield != null)
        {
            shield.gameObject.SetActive(IsPlayerInvulnerable());
        }

        if (dashCooldown > 0f)
        {
            dashCooldown -= Time.deltaTime;
        }
    }

    private void HandleInput()
    {
        if (StateMachine?.CurrentState is not PlayerMovementState)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RegisterClick();
            StateMachine.ChangeState(new PlayerAttackState(this));
        }

        if (Input.GetKeyDown(KeyCode.Space) && dashCooldown <= 0f)
        {
            StateMachine.ChangeState(new PlayerDodgeState(this));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StateMachine.ChangeState(new PlayerParryState(this));
        }
    }

    public override void UpdateMovement(Vector2 direction, float speed = 0f)
    {
        base.UpdateMovement(direction, speed);
        lastInputVector = direction;
    }

    public void RegisterClick()
    {
        var mouseScreenPos = Input.mousePosition;
        var mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        var mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        lastPosClick = mouseWorldPos;
    }

    public override void TakeDamage(Character origin)
    {
        if (IsPlayerInvulnerable())
        {
            if (StateMachine?.CurrentState is PlayerParryState)
            {
                lastPosClick = origin.transform.position;
                AddParryModifier();
                StateMachine.ChangeState(new PlayerAttackState(this, true));
            }

            return;
        }

        StateMachine?.ChangeState(new PlayerDamageState(this, GetPushValues(this, origin)));
        base.TakeDamage(origin);

        postHitInvulnerability = GameBalance.Config.invulnerabilityTime.ApplyPercentChange(FullUpgrade.invulnerableTime);
    }

    public void AddUpgrade(SO_Upgrade upgrade)
    {
        if (upgrade == null)
        {
            return;
        }

        Upgrades.Add(upgrade);
        RebuildUpgradeCache();
    }

    public void RemoveUpgrade(SO_Upgrade upgrade)
    {
        if (upgrade == null)
        {
            return;
        }

        Upgrades.Remove(upgrade);
        RebuildUpgradeCache();
    }

    public void AddParryModifier()
    {
        AddUpgrade(parryUpgrade);
    }

    public void RemoveParryModifier()
    {
        RemoveUpgrade(parryUpgrade);
    }

    private void Death()
    {
        StateMachine?.ChangeState(new PlayerDeathState(this));
    }

    public void DisableControl()
    {
        canControl = false;
    }

    public void EnableControl()
    {
        canControl = true;
    }

    public bool IsPlayerInvulnerable()
    {
        return (StateMachine?.CurrentState?.IsInvulnerable() ?? false) || postHitInvulnerability > 0f;
    }

    public override void ComboFinished()
    {
        base.ComboFinished();

        if (StateMachine?.CurrentState?.IsCountering() ?? false)
        {
            RemoveParryModifier();
            if (StateMachine.CurrentState is PlayerAttackState attackState)
            {
                attackState.StopCountering();
            }
        }
    }
}
