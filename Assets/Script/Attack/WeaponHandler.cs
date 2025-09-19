using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    private const string RotationTweenIdPrefix = "WeaponRotation";
    private const string MovementTweenIdPrefix = "WeaponMovement";

    [SerializeField] private SO_Weapon currentWeapon;
    [SerializeField] private float minComboInputDelay = 0.5f;

    private Weapon spawnedWeapon;
    private Coroutine strikeRoutine;
    private int currentStrikeIndex;
    private float comboTimer;

    public event Action OnStrikeFinished;
    public event Action OnStrikeCanceled;

    public SO_Weapon CurrentWeapon
    {
        get => currentWeapon;
        set => currentWeapon = value;
    }

    public void Initialize()
    {
        if (currentWeapon == null)
        {
            Debug.LogError($"{name} has no weapon assigned.");
            return;
        }

        GenerateWeapon();
        StoreWeapon();
    }

    private void Update()
    {
        if (comboTimer <= 0f)
        {
            return;
        }

        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0f)
        {
            currentStrikeIndex = 0;
        }
    }

    public void Attack(Character controller, Vector3 targetPoint)
    {
        if (strikeRoutine != null || spawnedWeapon == null)
        {
            return;
        }

        if (currentWeapon == null || currentWeapon.comboStrikes == null || currentWeapon.comboStrikes.Length == 0)
        {
            Debug.LogWarning($"{name} has no strikes configured.");
            return;
        }

        strikeRoutine = StartCoroutine(PerformStrike(controller, targetPoint));
    }

    public void InterruptCurrentAttack(bool notifyCanceled = true)
    {
        if (strikeRoutine != null)
        {
            StopCoroutine(strikeRoutine);
            strikeRoutine = null;
        }

        DOTween.Kill(RotationTweenIdPrefix + GetInstanceID());
        DOTween.Kill(MovementTweenIdPrefix + GetInstanceID());

        StoreWeapon();

        currentStrikeIndex = 0;
        comboTimer = 0f;

        if (notifyCanceled)
        {
            OnStrikeCanceled?.Invoke();
        }
    }

    public void GenerateWeapon()
    {
        if (spawnedWeapon != null)
        {
            Destroy(spawnedWeapon.gameObject);
        }

        if (currentWeapon.shape == null)
        {
            Debug.LogError($"Weapon {currentWeapon.name} is missing a shape prefab.");
            return;
        }

        spawnedWeapon = Instantiate(currentWeapon.shape, transform);
    }

    public void SetWeaponSize(Character controller)
    {
        if (spawnedWeapon == null)
        {
            return;
        }

        var scaleMultiplier = 1f.ApplyPercentChange(controller.FullUpgrade.weaponSizePercent);
        spawnedWeapon.transform.localScale = Vector3.one * scaleMultiplier;
    }

    public void StoreWeapon()
    {
        if (spawnedWeapon == null)
        {
            return;
        }

        spawnedWeapon.DisableCollider();
        spawnedWeapon.transform.localPosition = Vector3.left * 0.5f;
        spawnedWeapon.transform.localRotation = Quaternion.identity;
    }

    private IEnumerator PerformStrike(Character controller, Vector3 targetPoint)
    {
        comboTimer = 0f;

        var strike = Instantiate(currentWeapon.comboStrikes[currentStrikeIndex]);
        spawnedWeapon.EnableCollider();

        yield return ExecuteStrike(controller, strike, targetPoint);

        yield return new WaitForSeconds(ApplyWeaponSpeed(strike.delayEnd, controller));

        currentStrikeIndex++;
        if (currentStrikeIndex >= currentWeapon.comboStrikes.Length)
        {
            currentStrikeIndex = 0;
            controller.ComboFinished();
        }
        else
        {
            comboTimer = minComboInputDelay;
        }

        strikeRoutine = null;
        OnStrikeFinished?.Invoke();
    }

    private IEnumerator ExecuteStrike(Character controller, SO_Strike strike, Vector3 targetPoint)
    {
        var direction = (targetPoint - transform.position).normalized;
        var strikeDuration = ApplyWeaponSpeed(strike.duration, controller);

        var rotationTweenId = RotationTweenIdPrefix + GetInstanceID();
        var movementTweenId = MovementTweenIdPrefix + GetInstanceID();

        var left = -Vector2.Perpendicular(direction);
        var right = Vector2.Perpendicular(direction);

        float angle = 0f;
        switch (strike.type)
        {
            case StrikeMovementType.Thrust:
                angle = direction.SignedAngleZ();
                break;
            case StrikeMovementType.Swing:
            case StrikeMovementType.Spin:
                angle = left.SignedAngleZ();
                break;
            case StrikeMovementType.BackSwing:
                angle = right.SignedAngleZ();
                break;
        }

        var startupDelay = ApplyWeaponSpeed(strike.delayStart, controller);
        spawnedWeapon.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        spawnedWeapon.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(startupDelay);

        float rotationAmount = 0f;
        Vector2 movementAmount = Vector2.zero;

        switch (strike.type)
        {
            case StrikeMovementType.Thrust:
                movementAmount = direction * strike.moveAmount;
                break;
            case StrikeMovementType.Swing:
                rotationAmount = 180f;
                break;
            case StrikeMovementType.Spin:
                rotationAmount = 360f;
                break;
            case StrikeMovementType.BackSwing:
                rotationAmount = -180f;
                break;
        }

        spawnedWeapon.transform
            .DOLocalRotate(Vector3.forward * rotationAmount, strikeDuration, RotateMode.LocalAxisAdd)
            .SetId(rotationTweenId);

        spawnedWeapon.transform
            .DOLocalMove(movementAmount, strikeDuration)
            .SetId(movementTweenId);

        yield return new WaitForSeconds(strike.duration);
    }

    private float ApplyWeaponSpeed(float value, Character controller)
    {
        var finalValue = value.ApplyPercentChange(controller.FullUpgrade.weaponSpeedPercent);

        if (controller is Player player && player.StateMachine.CurrentState.IsCountering())
        {
            finalValue = finalValue.ApplyPercentChange(player.FullUpgrade.parryRetaliationSpeed);
        }

        return finalValue;
    }
}
