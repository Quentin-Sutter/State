using DG.Tweening;
using UnityEngine;

public class PlayerDamageState : PlayerState
{
    PushValues pushValues;

    private float timer;
    private float power;

    public PlayerDamageState(Player player, PushValues values) : base(player)
    {
        pushValues = values;
    } 

    public override void Enter()
    { 
        timer = pushValues.duration;
        power = pushValues.power;
        DOTween.To(() => power, x => power = x, 0.0f, pushValues.duration).SetEase(Ease.OutCubic).SetId("Damage" + player.GetInstanceID());
        FxManager.Instance.PlayVFX("Hit", player.transform.position);
        player.expressions.SetExpression(CharacterExpression.Expression.Hurt);
        player.weaponHandler.InterruptCurrentAttack();
    }

    public override void Exit()
    { 
        player.StopMovement();
        DOTween.Kill("Damage" + player.GetInstanceID());
    }

    public override void Update()
    {
        player.UpdateMovement(pushValues.direction, power);
         
        timer -= Time.deltaTime; 
        if (timer <= 0) player.StateMachine.ChangeState(new PlayerMovementState(player));
    }
}
