using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    // USP
   
    [SerializeField] GameObject[] animals;

    AnimalChara upperBodyChara;
    AnimalChara lowBodyChara;

    // Movement

    [HideInInspector] public Rigidbody2D rb;
    Vector2 moveInput;
    [SerializeField] float jumpForce = 100f;
    float jump = 1f;
    [HideInInspector] public bool isGrounded = true;
    bool isMoving = false;
    bool isSpecial = false;
    [HideInInspector] public bool isFlipped = false;

    // Combat

    //Combat
    [SerializeField] AttackBox attackBox;
    [SerializeField] float attackCooldown = 1.0f;
    [SerializeField] Transform attackBoxPosition;
    [SerializeField] Vector2 attackBoxSize;
    [SerializeField] LayerMask damageable;
    [SerializeField] int damage = 1;
    [SerializeField] int maxCombo = 3;
    [SerializeField] int maxLife = 2;

    int life = 0;
    bool isAlive = true;

    float attackTimer = 0.0f;
    bool isAttacking = false;

    // Stats

    [SerializeField] int maxLife;
    int life;
    bool isAlive = true;

    int comboCounter = 0;

    public void Awake()
    {
        life = maxLife;
        rb = GetComponent<Rigidbody2D>();
        RandomChangeBody();
        life = maxLife;
    }

    public void Update()
    {
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackCooldown)
            {
                isAttacking = false;
                attackTimer = 0.0f;
            }    
        }
    }

    public void FixedUpdate()
    {
        if (isMoving && !isSpecial)
        {
            if (moveInput.x <= 0f)
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                isFlipped = true;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                isFlipped = false;
            }

            if (!hasAttack)
                transform.Translate((moveInput.x > 0 ? moveInput.x : -moveInput.x) * Time.deltaTime * lowBodyChara.GetSpeed(), 0f, 0f);
        }
        if (isSpecial)
            isSpecial = lowBodyChara.PerformSpecialAttack(this);
    }

    public void OnMovement(CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = !context.canceled;
    }

    public void OnJump(CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            jump = context.ReadValue<float>();
            rb.velocity = new Vector2(rb.velocity.x, jump * jumpForce);
            jump = 0f;
            isGrounded = false;
        }
    }

    public void OnAttack(CallbackContext context)
    {
        if(context.performed)
        {
            //TODO mid air combat specific
            hasAttack = true;
            if(comboCounter < maxCombo)
                comboCounter++;
            //TODO trigger right animation combo
            //TODO reset combo counter when last animation combo is finish
            //TODO Check collision with animation event
            CheckAttackCollision();
        }
    }

    public void OnDebug(CallbackContext context)
    {
        if (context.started)
        {
            RandomChangeBody();
        }
    }

    public void OnSpecial(CallbackContext context)
    {
        if (!isSpecial && context.started)
        {
            isSpecial = true;
            lowBodyChara.InitiateSpecialAttack(this);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            isGrounded = true;
    }

    private void RandomChangeBody()
    {
        upperBodyChara = animals[Random.Range(0, animals.Length)].GetComponent<AnimalChara>();
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = upperBodyChara.GetUpperSprite();
        lowBodyChara = animals[Random.Range(0, animals.Length)].GetComponent<AnimalChara>();
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = lowBodyChara.GetLowSprite();
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive)
            return;
        life = Mathf.Clamp(life, 0, life - damage);

        if (life == 0)
            isAlive = false;
    }

    private void CheckAttackCollision()
    {
        Collider2D[] collideEnemies = Physics2D.OverlapBoxAll(attackBoxPosition.position, attackBoxSize, 0, damageable);

        foreach(Collider2D enemy in collideEnemies)
        {
            Enemy en = enemy.gameObject.GetComponent<Enemy>();
            
            if (en)
                en.takeDamage(damage, comboCounter);
        }
    }
    public void takeDamage(int damage)
    {
        Debug.Log("OUCH PLAYER");

        Mathf.Clamp(life, 0, life - damage);
        
        if (life == 0)
            isAlive = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(attackBoxPosition.position, attackBoxSize);
    }
}
