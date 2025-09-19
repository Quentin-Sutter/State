using DG.Tweening;
using UnityEngine;

public class EnemyDamageState : IEnemyState
{
    PushValues pushValues;

    private float timer;
    private float power;

    public EnemyDamageState(Enemy enemy, PushValues values)
    {
        pushValues = values;
    }

    public void Enter(Enemy enemy)
    {
        timer = pushValues.duration;
        power = pushValues.power;
        DOTween.To(() => power, x => power = x, 0.0f, pushValues.duration).SetEase(Ease.OutCubic).SetId("Damage" + enemy.GetInstanceID());
        FxManager.Instance.PlayVFX("Hit", enemy.transform.position);
        enemy.expressions.SetExpression(CharacterExpression.Expression.Hurt);
        enemy.weaponHandler.InterruptCurrentAttack();
    }

    public void Exit(Enemy enemy)
    {
        enemy.StopMovement();
        DOTween.Kill("Damage" + enemy.GetInstanceID());
    }

    public void Update(Enemy enemy)
    {
        enemy.UpdateMovement(pushValues.direction, power);
        timer -= Time.deltaTime;
        if (timer <= 0) enemy.ChangeState(new EnemyChaseState());
    }
}
