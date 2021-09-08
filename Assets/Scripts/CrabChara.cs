using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabChara : AnimalChara
{
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] int nbMissileFired = 5;
    [SerializeField] int missileSpeed = 5;
    float fireTimer = 0f;
    bool HasFired = false;

    public override bool PerformSpecialAttack(Player player)
    {
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
    public override void InitiateSpecialAttack(Player player)
    {
        HasFired = false;
        fireTimer = nbMissileFired * fireRate;
    }

    private void LaunchMissile()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject enemy = null;
        foreach (GameObject go in enemies)
        {
            if (!go.GetComponent<Enemy>().hasBeenTargeted)
            {
                go.GetComponent<Enemy>().hasBeenTargeted = true;
                enemy = go;
                break;
            }
        }
        if (enemy == null)
            return;

        Instantiate(missilePrefab, transform.position, Quaternion.identity).GetComponent<PlayerMissile>().InitiateMissile
            (specialDamage, missileSpeed, enemy);
    }
}