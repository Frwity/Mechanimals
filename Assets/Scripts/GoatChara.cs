using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatChara : AnimalChara
{
    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashTime = 1f;
    float dashTimer = 0f;

    public override bool PerformSpecialAttack(Player player)
    {
        dashTimer -= Time.deltaTime;
        player.rb.velocity = new Vector2(player.isFlipped ? -dashForce : dashForce, player.rb.velocity.y);
        if (dashTimer < 0f)
        {
            player.rb.velocity = new Vector2(0, player.rb.velocity.y);
            return false;
        }
        return true;
    }

    public override void InitiateSpecialAttack(Player player)
    {
        dashTimer = dashTime;
    }
}
