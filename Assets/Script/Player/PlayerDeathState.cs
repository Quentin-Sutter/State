using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        ChangeColor(Color.black);
        player.RemoveControlPlayer();
        GameManager.Instance.GameLosed();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {

    }
}
