using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearChara : AnimalChara
{
    [SerializeField] float jumpUpForce = 30f;
    [SerializeField] float jumpDownForce = 50f;
    [SerializeField] float jumpUpTime = 0.5f;
    float jumpUpTimer = 0f;

    public override bool PerformSpecialAttack(Player player)
    {
        jumpUpTimer -= Time.deltaTime;
        if (jumpUpTimer < 0f)
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, -jumpDownForce);
        }
        else
        {
            player.isGrounded = false;
            player.rb.velocity = new Vector2(player.rb.velocity.x, jumpUpForce);
        }

        if (player.isGrounded)
        {
            player.specialBoxPosition.SetActive(true);

            return false;
        }

        return true;
    }
    public override void InitiateSpecialAttack(Player player)
    {
        jumpUpTimer = jumpUpTime;
    }
}
