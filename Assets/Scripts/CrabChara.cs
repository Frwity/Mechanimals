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
            return false;
        return true;
    }
    public override void InitiateSpecialAttack(Player _player)
    {
        player = _player;
        HasFired = false;
        fireTimer = nbMissileFired * fireRate;
    }

    private void LaunchMissile()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject enemy = enemies[Random.Range(0, enemies.Length)];

        if (enemy == null)
        {
            HasFired = true;
            fireTimer = -1f;
            return;
        }

        Instantiate(missilePrefab, pos, Quaternion.identity).GetComponent<PlayerMissile>().InitiateMissile
            (specialDamage, missileSpeed, enemy, (transform.position.x < enemy.transform.position.x ? player.knockBackDirection : new Vector2(-player.knockBackDirection.x, player.knockBackDirection.y)) * specialKnockbackForce);
    }
}