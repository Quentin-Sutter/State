using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class WeaponHandler : MonoBehaviour
{
    public SO_Weapon currentWeapon;

    Weapon instanciedWeapon;

    Coroutine _strikeRoutine;

    private int currentStrikeIndex = 0;

    public float timeMinForCombo = 0.5f;
    float comboTimer;

    public event Action OnStrikeFinished;
    public event Action OnStrikeCanceled; 

    public void Initialize ()
    {
        GenerateWeapon();
        StoreWeapon(); 
    }

    void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f) currentStrikeIndex = 0;
        }
    }

    public void Attack(Character controller, Vector3 targetPoint)
    { 
        if (_strikeRoutine != null) return;

        _strikeRoutine = StartCoroutine(DoStrike(controller, targetPoint));
    }

     
    IEnumerator DoStrike(Character controller, Vector3 targetPoint)
    {
        comboTimer = 0f;
        SO_Strike strike = Instantiate(currentWeapon.comboStrikes[currentStrikeIndex]);
        instanciedWeapon.EnableCollider();


        yield return PerformHit(controller, strike, instanciedWeapon, targetPoint);
 
        yield return new WaitForSeconds(ApplyWeaponSpeed(strike.delayEnd, controller)); 

        currentStrikeIndex++;
        if (currentStrikeIndex > currentWeapon.comboStrikes.Count() - 1)
        {
            currentStrikeIndex = 0;
            controller.ComboFinished();
        }
        else comboTimer = timeMinForCombo;

        _strikeRoutine = null;
        OnStrikeFinished?.Invoke();
    }

    IEnumerator PerformHit(Character controller, SO_Strike strike, Weapon weapon, Vector3 targetPoint)
    {
        Vector2 dir = (Vector2)(targetPoint - transform.position).normalized;

        float weaponSpeedSeconds = ApplyWeaponSpeed(strike.duration, controller);

        // IDs par instance pour kill ciblé
        string rotId = "WeaponRotation" + gameObject.GetInstanceID();
        string movId = "WeaponMovement" + gameObject.GetInstanceID();

        // Orientation
        Vector2 left = -Vector2.Perpendicular(dir);
        Vector2 right = Vector2.Perpendicular(dir);
        float angle = 0f;

        switch (strike.type)
        {
            case StrikeMovementType.Thrust: angle = dir.SignedAngleZ(); break;
            case StrikeMovementType.Swing: angle = left.SignedAngleZ(); break;
            case StrikeMovementType.Spin: angle = left.SignedAngleZ(); break;
            case StrikeMovementType.BackSwing: angle = right.SignedAngleZ(); break;
        }
        
        float delayStart = ApplyWeaponSpeed(strike.delayStart, controller);
        weapon.transform.rotation = Quaternion.Euler(Vector3.forward * angle); 
        weapon.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(delayStart);


        // Mouvement
        float rotationAmount = 0f;
        Vector2 moveAmount = Vector2.zero;
        switch (strike.type)
        {
            case StrikeMovementType.Thrust: moveAmount = dir * strike.moveAmount; break;
            case StrikeMovementType.Swing: rotationAmount = 180f; break;
            case StrikeMovementType.Spin: rotationAmount = 360f; break;
            case StrikeMovementType.BackSwing: rotationAmount = -180f; break;
        }

   

        weapon.transform
              .DOLocalRotate(Vector3.forward * rotationAmount, weaponSpeedSeconds, RotateMode.LocalAxisAdd) 
              .SetId(rotId);

        weapon.transform
              .DOLocalMove(moveAmount, weaponSpeedSeconds) 
              .SetId(movId);

        // Attente = durée déclarée du strike (pas forcément égale aux durées DOTween si tu varies)
        yield return new WaitForSeconds(strike.duration);
    }

    // À appeler quand le perso prend un coup
    public void InterruptCurrentAttack(bool notifyCanceled = true)
    {
        // Stop la coroutine principale d’attaque
        if (_strikeRoutine != null)
        {
            StopCoroutine(_strikeRoutine);
            _strikeRoutine = null;
        }

        // Tuer tweens en cours (IDs uniques)
        DOTween.Kill("WeaponRotation" + gameObject.GetInstanceID());
        DOTween.Kill("WeaponMovement" + gameObject.GetInstanceID());

        StoreWeapon();

        // Reset combo/timers 
        currentStrikeIndex = 0;
        comboTimer = 0f;

        if (notifyCanceled) OnStrikeCanceled?.Invoke();
    } 

    public void GenerateWeapon ()
    {
        instanciedWeapon = Instantiate(currentWeapon.shape, transform);
    }

    public void SetWeaponSize (Character controller)
    {
        instanciedWeapon.transform.localScale = Vector3.one * 1.0f.ApplyPercentChange(controller.fullUpgrade.weaponSizePercent);
    }

    public void StoreWeapon()
    { 
        instanciedWeapon.DisableCollider();
        instanciedWeapon.transform.localPosition = Vector3.left * 0.5f;
        instanciedWeapon.transform.localRotation = Quaternion.identity;
    }

    float ApplyWeaponSpeed (float value, Character controller)
    {
        float finalValue = value.ApplyPercentChange(controller.fullUpgrade.weaponSpeedPercent);
        if (controller is Player player && player.StateMachine.CurrentState.IsCountering())
        {
            finalValue = finalValue.ApplyPercentChange(player.fullUpgrade.parryRetaliationSpeed);
        }

        return finalValue;
    }
}