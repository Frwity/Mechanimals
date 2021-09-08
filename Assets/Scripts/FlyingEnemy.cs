using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] GameObject missilePrefab;
    [SerializeField] float missileSpeed = 3f;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] float timeBeforeReFlying = 1.0f;
    float flyingTimer= 0.0f;
    bool isFlying = true;
    bool asReachTargetWaypoint = true;
    Vector3 waypointTarget;
    [HideInInspector] public int waypointNo = 0;

    public override void Start()
    {
        base.Start();

    }

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
        if (!isWaiting && !isAttacking && asReachTargetWaypoint)
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
                Debug.Log(waypointNo);
                waypointNo = (waypointNo + Random.Range(0, waypoints.Count)) % waypoints.Count;
                Debug.Log(waypointNo);
                waypointTarget = waypoints[waypointNo].transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                target = null;
                asReachTargetWaypoint = false;
            }
        }
        else // movement
        {
            transform.Translate((waypointTarget - transform.position).normalized * speed * Time.deltaTime);
            if ((waypointTarget - transform.position).magnitude < 1f)
                asReachTargetWaypoint = true;
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
