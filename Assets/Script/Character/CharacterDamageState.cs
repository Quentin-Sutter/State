using DG.Tweening;
using UnityEngine;

public class CharacterDamageState : CharacterState
{
    private readonly PushValues pushValues;
    private float timer;
    private float power;
    private string tweenId;

    public CharacterDamageState(CharacterStateMachine stateMachine, PushValues values) : base(stateMachine)
    {
        pushValues = values;
    }

    public override void Enter(CharacterContext context)
    {
        timer = pushValues.duration;
        power = pushValues.power;
        tweenId = "Damage" + Character.GetInstanceID();

        DOTween.To(() => power, x => power = x, 0.0f, pushValues.duration)
            .SetEase(Ease.OutCubic)
            .SetId(tweenId);

        FxManager.Instance.PlayVFX("Hit", Character.transform.position);
        SetExpression(CharacterExpression.Expression.Hurt);
        Character.WeaponHandler.InterruptCurrentAttack();
    }

    public override void Update(CharacterContext context)
    {
        Character.UpdateMovement(pushValues.direction, power);

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            StateMachine.ChangeState(new CharacterMovementState(StateMachine));
        }
    }

    public override void Exit(CharacterContext context)
    {
        StopMovement();
        if (!string.IsNullOrEmpty(tweenId))
        {
            DOTween.Kill(tweenId);
        }
    }
}
