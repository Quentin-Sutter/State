using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{
    [Header("Scripts")]
    public Health health;
    public WeaponHandler weaponHandler;
    public CharacterExpression expressions;

    [Header("Components")]
    public Rigidbody2D rb;
    public SpriteRenderer skin;

    [Header("Upgrades")]
    public UpgradeStats fullUpgrade;
    public List<SO_Upgrade> upgrades; 

    public virtual void Init ()
    {
        for (int i = 0; i < weaponHandler.currentWeapon.upgradesOnStart.Length; i++) upgrades.Add(weaponHandler.currentWeapon.upgradesOnStart[i]);
        weaponHandler.Initialize();
        CreateFullUpgrade();
    }

    public virtual void TakeDamage(Character origin)
    {
        float baseAmount = origin.weaponHandler.currentWeapon.damage;
        float finalAmount = baseAmount.ApplyPercentChange(origin.fullUpgrade.weaponDamagePercent);
        health.TakeDamage((int)finalAmount); 
    }

    public virtual void UpdateMovement(Vector2 direction, float speed = 0.0f)
    {
        float currentSpeed = GameBalance.Config.baseMoveSpeed;
        if (speed != 0.0f) currentSpeed = speed;
        currentSpeed = currentSpeed.ApplyPercentChange(fullUpgrade.moveSpeedPercent);
        Vector2 targetVelocity = direction * currentSpeed;

        rb.linearVelocity = targetVelocity; 
    }

    public void StopMovement ()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.tag == "Player")
        {
            if (collision.tag == "Enemy")
            {
                collision.gameObject.GetComponent<IDamageable>().TakeDamage(this);
            }
        }
        else
        {
            if (collision.tag == "Player")
            {
                collision.gameObject.GetComponent<IDamageable>().TakeDamage(this);
            }
        }

    }

    public virtual void ComboFinished()
    {
        weaponHandler.StoreWeapon();
    }

    public virtual void StrikeFinished ()
    {

    }

    public void CreateFullUpgrade()
    {
        fullUpgrade = new UpgradeStats();

        foreach (SO_Upgrade u in upgrades)
        {
            fullUpgrade.Add(u);
        }

        weaponHandler.SetWeaponSize(this);
    }

    public virtual PushValues GetPushValues (Character receiver, Character origin)
    {
        PushValues ret = new PushValues();

        Vector2 direction = receiver.transform.position - origin.transform.position;

        float force = origin.weaponHandler.currentWeapon.pushPower.ApplyPercentChange(origin.fullUpgrade.pushPowerPercent - receiver.fullUpgrade.pushResistance);

        float stunTime = origin.weaponHandler.currentWeapon.pushDuration.ApplyPercentChange(origin.fullUpgrade.pushDurationPercent - receiver.fullUpgrade.pushResistance);

        ret.direction = direction.normalized;
        ret.power = force;
        ret.duration = stunTime;

        return ret;
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


