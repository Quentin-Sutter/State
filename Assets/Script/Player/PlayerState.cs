using UnityEngine;
using DG.Tweening;

public abstract class PlayerState
{
    protected Player player;

    public PlayerState(Player player)
    {
        this.player = player;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

    protected void StopPlayerMovement()
    {
        player.StopMovement();
    }

    public void ChangeColor(Color color, float time = 0.0f)
    {
        if (time > 0.0f)
        {
            player.skin.DOColor(color, time);
        }
        else
        {
            player.skin.color = color;
        }
    }

    public virtual bool IsInvulnerable()
    {
        return false;
    }

    public virtual bool IsCountering ()
    {
        return false;
    }
}
