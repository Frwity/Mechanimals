using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [HideInInspector] GameObject[] waypoints;
    [SerializeField] GameObject missilePrefab;
    [SerializeField] float missileSpeed = 3f;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] float timeBeforeReFlying = 1.0f;
    float flyingTimer= 0.0f;
    bool isFlying = true;
    bool asReachTargetWaipoint = false; 

    public override void Update()
    {
        if (!isAlive)
        {
            Destroy(gameObject, timeToDie);
            return;
        }
        if (!isFlying)
        {
            flyingTimer += Time.deltaTime;
            if (flyingTimer >= timeBeforeReFlying)
            {
                isFlying = true;
                flyingTimer = 0.0f;
            }
            else
                return;
        }
        if (target == null)
        {
            target = worldMananger.GetRandomPlayer(transform.position);
            return;
        }
        if (!isWaiting && !isAttacking)
        {
            isAttacking = true;
            Attack();
        }
        else if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                isAttacking = false;
                attackTimer = 0.0f;
                waitTimer = 0.0f;
                isWaiting = true;
            }
        }
        else if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0.0f;
                target = worldMananger.GetClosestPlayer(transform.position);
            }
        }
        else // movement
        {
            //transform.Translate((transform.position) speed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        if (!isAlive)
            return;
        Invoke("LaunchMissile", 0);
        Invoke("LaunchMissile", fireRate);
        Invoke("LaunchMissile", fireRate * 2);

    }

    public override void TakeDamage(int damage, int comboNum, Vector2 knockbackVelocity)
    {
        base.TakeDamage(damage, comboNum, knockbackVelocity);
        GetComponent<Rigidbody2D>().gravityScale = 1f;
    }

    private void LaunchMissile()
    {
        Instantiate(missilePrefab, transform.position, Quaternion.identity).GetComponent<EnemyMissile>().InitiateMissile
            (damage, missileSpeed, (target.transform.position - transform.position).normalized);
    }
}
