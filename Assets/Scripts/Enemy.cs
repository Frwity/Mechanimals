using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public List<GameObject> waypoints;
    [HideInInspector] public WorldManager worldMananger;
    [HideInInspector] public Arena arena = null;

    [SerializeField] protected float attackCooldown = 1.0f;
    [SerializeField] protected int damage = 1;
    [SerializeField] Transform attackBoxPosition = null;
    [SerializeField] Vector2 attackBoxSize;
    [SerializeField] protected LayerMask damageable;
    
    // Stats

    [SerializeField] protected int maxlife = 6;
    protected int life;
    protected bool isAlive = true;

    // Movement

    [SerializeField] protected float speed = 3f;
    Rigidbody2D rb;

    //Combat

    protected GameObject target;

    [SerializeField] protected float waitTime = 1.0f;
    protected float waitTimer = 0f;
    protected bool isWaiting = false;
    protected float attackTimer = 0.0f;
    protected bool isAttacking = false;
    [SerializeField] float range = 4.0f;
    [SerializeField] protected float timeToDie = 1f;

    [SerializeField] protected Vector2 knockBackDirection;
    [SerializeField] protected float knockbackForce;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        life = maxlife;
        target = worldMananger.GetClosestPlayer(transform.position);
    }

    public virtual void Update()
    {
        if (!isAlive)
        {
            Destroy(gameObject, timeToDie);
            return;
        }
        if (target == null || !target.GetComponent<Player>().isAlive)
        {
            target = worldMananger.GetClosestPlayer(transform.position);
            return;
        }
        if (!isWaiting && !isAttacking && (target.transform.position - transform.position).magnitude < range)
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
        else
        {
            if (target.transform.position.x < transform.position.x)
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            else
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            int layerMask = 1 << 6; // player
            layerMask = 1 << 7; // enemy
            layerMask = 1 << 9; // Wall
            layerMask = ~layerMask;

            if (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y, layerMask))
                transform.Translate(speed * Time.deltaTime, 0f, 0f);
            else
                transform.Translate(-speed * 3f * Time.deltaTime, 0f, 0f);
        }
    }

    public virtual void TakeDamage(int damage, int comboNum, Vector3 knockbackVelocity)
    {
        life = Mathf.Clamp(life, 0, life - damage);
        GetComponent<Rigidbody2D>().velocity = knockbackVelocity;
        if (life == 0)
        {
            if (arena)
                arena.AddEnemyKill();
            isAlive = false;
        }
    }

    private void Attack()
    {
        if (!isAlive)
            return;
        Collider2D[] collidePlayers = Physics2D.OverlapBoxAll(attackBoxPosition.position, attackBoxSize, 0, damageable);

        foreach (Collider2D player in collidePlayers)
        {
            Vector2 knockbackVelocity = (transform.position.x < player.transform.position.x ? knockBackDirection : new Vector2(-knockBackDirection.x, knockBackDirection.y)) * knockbackForce;
            Player p = player.gameObject.GetComponent<Player>();

            if (p)
                p.TakeDamage(damage);
        }
    }

    public void OnDrawGizmos()
    {
        if (attackBoxPosition)
            Gizmos.DrawCube(attackBoxPosition.position, attackBoxSize);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            rb.velocity = Vector3.zero;
            rb.sharedMaterial.bounciness = 0f;
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            rb.sharedMaterial.bounciness = 0.5f;
        }
    }
}
