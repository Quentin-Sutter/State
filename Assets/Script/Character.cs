using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{
    [Header("Scripts")]
    [SerializeField] private Health health;
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private CharacterExpression expressions;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer skin;

    [Header("Upgrades")]
    [SerializeField] private List<SO_Upgrade> upgrades = new();

    public Health Health => health;
    public WeaponHandler WeaponHandler => weaponHandler;
    public CharacterExpression Expressions => expressions;
    public Rigidbody2D Rigidbody => rb;
    public SpriteRenderer Skin => skin;
    public UpgradeStats FullUpgrade { get; private set; } = new();

    protected List<SO_Upgrade> Upgrades => upgrades;

    public void SetUpgrades(IEnumerable<SO_Upgrade> newUpgrades)
    {
        upgrades = newUpgrades != null ? new List<SO_Upgrade>(newUpgrades) : new List<SO_Upgrade>();
        RebuildUpgradeCache();
    }

    protected virtual void Awake()
    {
        health ??= GetComponent<Health>();
        weaponHandler ??= GetComponent<WeaponHandler>();
        expressions ??= GetComponent<CharacterExpression>();
        rb ??= GetComponent<Rigidbody2D>();
        skin ??= GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Init()
    {
        if (weaponHandler == null)
        {
            Debug.LogError($"{name} has no WeaponHandler configured.");
            return;
        }

        if (upgrades == null)
        {
            upgrades = new List<SO_Upgrade>();
        }

        if (weaponHandler.CurrentWeapon != null && weaponHandler.CurrentWeapon.upgradesOnStart != null)
        {
            foreach (var upgrade in weaponHandler.CurrentWeapon.upgradesOnStart)
            {
                if (upgrade != null)
                {
                    upgrades.Add(upgrade);
                }
            }
        }

        weaponHandler.Initialize();
        RebuildUpgradeCache();
    }

    public virtual void TakeDamage(Character origin)
    {
        if (origin?.WeaponHandler?.CurrentWeapon == null)
        {
            return;
        }

        var baseAmount = origin.WeaponHandler.CurrentWeapon.damage;
        var finalAmount = baseAmount.ApplyPercentChange(origin.FullUpgrade.weaponDamagePercent);
        health.TakeDamage(Mathf.RoundToInt(finalAmount));
    }

    public virtual void UpdateMovement(Vector2 direction, float speed = 0f)
    {
        var currentSpeed = Mathf.Approximately(speed, 0f) ? GameBalance.Config.baseMoveSpeed : speed;
        currentSpeed = currentSpeed.ApplyPercentChange(FullUpgrade.moveSpeedPercent);
        var targetVelocity = direction * currentSpeed;

        rb.linearVelocity = targetVelocity;
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<IDamageable>(out var damageable))
        {
            return;
        }

        if (CompareTag("Player") && collision.CompareTag("Enemy"))
        {
            damageable.TakeDamage(this);
        }
        else if (!CompareTag("Player") && collision.CompareTag("Player"))
        {
            damageable.TakeDamage(this);
        }
    }

    public virtual void ComboFinished()
    {
        weaponHandler.StoreWeapon();
    }

    public virtual void StrikeFinished()
    {
    }

    public void RebuildUpgradeCache()
    {
        FullUpgrade = new UpgradeStats();

        if (upgrades == null)
        {
            return;
        }

        foreach (var upgrade in upgrades)
        {
            if (upgrade != null)
            {
                FullUpgrade.Add(upgrade);
            }
        }

        weaponHandler?.SetWeaponSize(this);
    }

    public virtual PushValues GetPushValues(Character receiver, Character origin)
    {
        var direction = receiver.transform.position - origin.transform.position;
        var force = origin.WeaponHandler.CurrentWeapon.pushPower.ApplyPercentChange(
            origin.FullUpgrade.pushPowerPercent - receiver.FullUpgrade.pushResistance);
        var stunTime = origin.WeaponHandler.CurrentWeapon.pushDuration.ApplyPercentChange(
            origin.FullUpgrade.pushDurationPercent - receiver.FullUpgrade.pushResistance);

        return new PushValues
        {
            direction = direction.normalized,
            power = force,
            duration = stunTime
        };
    }
}

public class PushValues
{
    public Vector2 direction;
    public float power;
    public float duration;
}

public class UpgradeStats
{
    public float weaponSpeedPercent = 0f;
    public float weaponDamagePercent = 0f;
    public float weaponSizePercent = 0f;
    public float pushPowerPercent = 0f;
    public float pushDurationPercent = 0f;
    public float moveSpeedPercent = 0f;
    public float pushResistance = 0f;
    public float invulnerableTime = 0f;
    public float dodgeDuration = 0f;
    public float dodgeSpeed = 0f;
    public float parryDuration = 0f;
    public float parryDamage = 0f;
    public float parryRetaliationSpeed = 0f;

    public void Add(SO_Upgrade upgrade)
    {
        weaponSpeedPercent += upgrade.weaponSpeedPercent;
        weaponDamagePercent += upgrade.weaponDamagePercent;
        weaponSizePercent += upgrade.weaponSizePercent;
        pushPowerPercent += upgrade.pushPowerPercent;
        pushDurationPercent += upgrade.pushDurationPercent;
        moveSpeedPercent += upgrade.moveSpeedPercent;
        pushResistance += upgrade.pushResistance;
        invulnerableTime += upgrade.invulnerabilityTime;
        dodgeDuration += upgrade.dodgeDuration;
        dodgeSpeed += upgrade.dodgeSpeed;
        parryDuration += upgrade.parryDuration;
        parryDamage += upgrade.parryDamage;
        parryRetaliationSpeed += upgrade.parryRetaliationSpeed;
    }
}
