using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public WorldManager worldMananger;
    [HideInInspector] public Arena arena = null;
    [SerializeField] int life = 10;
    [SerializeField] float attackCooldown = 1.0f;
    [SerializeField] int damage = 1;
    [SerializeField] Transform attackBoxPosition = null;
    [SerializeField] Vector2 attackBoxSize;
    [SerializeField] LayerMask damageable;

    // Stats

    [SerializeField] int maxlife = 6;
    int life;
    bool isAlive = true;

    // Movement

    [SerializeField] float speed = 3f;

    //Combat

    GameObject target;

    [SerializeField] AttackBox attackBox;
    [SerializeField] float waitTime = 1.0f;
    float waitTimer = 0f;
    bool isWaiting = false;
    [SerializeField] float attackCooldown = 1.0f;
    float attackTimer = 0.0f;
    bool isAttacking = false;
    [SerializeField] float range = 4.0f;
    [SerializeField] float timeToDie = 1f;

    public void Start()
    {
        life = maxlife;
        target = worldMananger.GetClosestPlayer(transform.position);
    }

    public void Update()
    {
        if (!isAlive)
        {
            Destroy(gameObject, timeToDie);
            return;
        }
        if (target == null)
        {
            target = worldMananger.GetClosestPlayer(transform.position);
            return;
        }
        if (!isWaiting && !isAttacking && (target.transform.position - transform.position).magnitude < range)
        {
            isAttacking = true;
            attackBox.Attack();
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

            transform.Translate(speed * Time.deltaTime, 0f, 0f);
        }
    }

    public void TakeDamage(int damage, int comboNum)
    {
        if (!isAlive)
            return;
        life = Mathf.Clamp(life, 0, life - damage);
        if (life == 0)
        {
            isAlive = false;
            if (arena)
                arena.AddEnemyKill();
        }
    public void takeDamage(int damage, int comboNum)
    {
        Debug.Log("OUCH");
        Mathf.Clamp(life, 0, life - damage);

        if (life == 0)
            isAlive = false;
    }

    private void Attack()
    {
        Collider2D[] collidePlayers = Physics2D.OverlapBoxAll(attackBoxPosition.position, attackBoxSize, 0, damageable);

        foreach (Collider2D player in collidePlayers)
        {
            Player p = player.gameObject.GetComponent<Player>();

            if (p)
                p.takeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(attackBoxPosition.position, attackBoxSize);
    }
}
