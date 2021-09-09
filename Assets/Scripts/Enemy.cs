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
    [HideInInspector] public int life;
    protected bool isAlive = true;

    // Movement

    [SerializeField] protected float speed = 3f;
    protected Rigidbody2D rb;

    //Combat

    protected GameObject target;

    [SerializeField] protected float waitTime = 1.0f;
    protected float waitTimer = 0f;
    protected bool isWaiting = false;
    protected float attackTimer = 0.0f;
    protected bool isAttacking = false;
    [SerializeField] float range = 4.0f;

    [SerializeField] float stunTime = 1.0f;
    float stunTimer = 0.0f;
    [SerializeField] protected float timeToDie = 1f;

    [SerializeField] protected Vector2 knockBackDirection;
    [SerializeField] protected float knockbackForce;

    //Animation
    Animator anim;

    protected bool isHit = false;

    //Audio
    [SerializeField] public AudioClip[] enemySounds;

    protected AudioSource audioSource;

    public void Awake()
    {
        waypoints = new List<GameObject>();
    }

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        life = maxlife;
        target = worldMananger.GetClosestPlayer(transform.position);
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void Update()
    {
        if (!isAlive)
        {
            Destroy(gameObject, timeToDie);
            return;
        }
        if (isHit)
        {
            stunTimer += Time.deltaTime;
            if (stunTimer >= stunTime)
            {
                isHit = false;
                stunTimer = 0.0f;
            }
            else
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
            anim.SetBool("IsMoving", false);
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
            anim.SetBool("IsMoving", true);
            if (target.transform.position.x < transform.position.x)
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            else
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            int layerMask = 1 << 3; // wall
            layerMask |= (1 << 6); // player
            layerMask |= (1 << 7); // enemy
            layerMask |= (1 << 8); // flyningenemy
            layerMask |= (1 << 9); // border

            layerMask = ~layerMask;

            if (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y, layerMask))
                transform.Translate(speed * Time.deltaTime, 0f, 0f);
            else
                transform.Translate(-speed * 1.5f * Time.deltaTime, 0f, 0f);
        }
    }

    public virtual void TakeDamage(int damage, int comboNum, Vector3 knockbackVelocity)
    {
        life = Mathf.Clamp(life, 0, life - damage);
        GetComponent<Rigidbody2D>().velocity = knockbackVelocity;
        anim.SetBool("IsMoving", false);
        anim.SetTrigger("Hit");
        if (life == 0)
        {
            audioSource.PlayOneShot(enemySounds[1]);
            if (arena)
                arena.AddEnemyKill();
            isAlive = false;
        }
        else
            isHit = true;
    }

    private void Attack()
    {
        if (!isAlive)
            return;
        Collider2D[] collidePlayers = Physics2D.OverlapBoxAll(attackBoxPosition.position, attackBoxSize, 0, damageable);
        anim.SetBool("IsMoving", false);
        anim.SetTrigger("Attack");

        foreach (Collider2D player in collidePlayers)
        {
            audioSource.PlayOneShot(enemySounds[0]);
            Vector2 knockbackVelocity = (transform.position.x < player.transform.position.x ? knockBackDirection : new Vector2(-knockBackDirection.x, knockBackDirection.y)) * knockbackForce;
            Player p = player.gameObject.GetComponent<Player>();

            if (p)
                p.TakeDamage(damage, knockbackVelocity);
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
            if (rb == null)
                return;

            rb.velocity = Vector3.zero;
            rb.sharedMaterial.bounciness = 0f;
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            if (rb == null)
                return;

            rb.sharedMaterial.bounciness = 0.5f;
        }
    }
}
