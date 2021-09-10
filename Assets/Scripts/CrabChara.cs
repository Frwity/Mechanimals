using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabChara : AnimalChara
{
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] int nbMissileFired = 5;
    [SerializeField] int missileSpeed = 5;
    Vector2 pos;
    float fireTimer = 0f;
    bool HasFired = false;
    Player player;
    public override bool PerformSpecialAttack(Player player)
    {
        pos = player.transform.position;
        fireTimer -= Time.deltaTime;
        if (!HasFired)
        {
            for (int i = 0; i < nbMissileFired; ++i)
                Invoke("LaunchMissile", i * fireRate);
            HasFired = true;
        }
        if (fireTimer < 0f)
        {
            player.GetComponent<Rigidbody2D>().simulated = true;
            return false;
        }
        return true;
    }
    public override void InitiateSpecialAttack(Player _player)
    {
        player = _player;
        player.GetComponent<Rigidbody2D>().simulated = false;
        HasFired = false;
        fireTimer = nbMissileFired * fireRate;
    }

    private void LaunchMissile()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length <= 0)
            return;
        GameObject enemy = enemies[Random.Range(0, enemies.Length)];

        if (enemy == null)
        {
            HasFired = true;
            fireTimer = -1f;
            return;
        }

        Instantiate(missilePrefab, pos, Quaternion.identity).GetComponent<PlayerMissile>().InitiateMissile
            (specialDamage, missileSpeed, enemy, player.knockBackDirection * specialKnockbackForce);
    }
}