using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private bool fromParry;
    private bool continueCombo;
    System.Action onStrikeFinishedHandler;

    float timeBeforeRegisterClick = 0.2f;

    public PlayerAttackState(Player player, bool fromParry = false) : base(player)
    {
        this.fromParry = fromParry;
    }

    void StrikeFinished(Player player)
    {
        if (continueCombo)
        {
            player.StateMachine.ChangeState(new PlayerAttackState(player, fromParry)); 
        }
        else
        {
            player.StateMachine.ChangeState(new PlayerMovementState(player)); 
        }
        continueCombo = false;
    }

    public override void Enter()
    {
        continueCombo = false;
        player.StopMovement();

        player.weaponHandler.Attack(player, player.lastPosClick);
        onStrikeFinishedHandler = () => StrikeFinished(player);
        player.weaponHandler.OnStrikeFinished += onStrikeFinishedHandler;
        player.expressions.SetExpression(CharacterExpression.Expression.Angry); 
    }

    public override void Update()
    {
        if (timeBeforeRegisterClick > 0.0f)
        {
            timeBeforeRegisterClick -= Time.deltaTime;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                continueCombo = true;
                player.RegisterClick();
            }
        }
    }

    public override void Exit()
    {
        player.weaponHandler.OnStrikeFinished -= onStrikeFinishedHandler;
        if (continueCombo == false) player.ComboFinished();
    }

    public override bool IsCountering()
    {
        return fromParry;
    }

    public override bool IsInvulnerable()
    {
        return fromParry;
    }

    public void StopCountering ()
    {
        fromParry = false;
    }
}
