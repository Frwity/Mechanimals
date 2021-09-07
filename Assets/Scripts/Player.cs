using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    // USP
   
    [SerializeField] GameObject[] animals;

    AnimalChara upperBody;
    AnimalChara lowBody;

    // Movement

    public Rigidbody2D rb;
    [SerializeField] float jumpForce = 100f;

    //Combat
    [SerializeField] float attackCooldown = 1.0f;
    [SerializeField] Transform attackBoxPosition;
    [SerializeField] Vector2 attackBoxSize;
    [SerializeField] LayerMask damageable;
    [SerializeField] int damage = 1;
    [SerializeField] int maxCombo = 3;

    float attackTimer = 0.0f;

    bool hasAttack = false;

    Vector2 moveInput;
    float jump = 1f;
    public bool isGrounded = true;
    bool isMoving = false;
    bool isSpecial = false;
    public bool isFlipped = false;

    int comboCounter = 0;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        RandomChangeBody();
    }

    public void Update()
    {
        if (hasAttack)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackCooldown)
            {
                hasAttack = false;
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
                transform.Translate((moveInput.x > 0 ? moveInput.x : -moveInput.x) * Time.deltaTime * lowBody.GetSpeed(), 0f, 0f);
        }
        if (isSpecial)
            isSpecial = lowBody.PerformSpecialAttack(this);
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
            lowBody.InitiateSpecialAttack(this);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            isGrounded = true;
    }

    private void RandomChangeBody()
    {
        upperBody = animals[Random.Range(0, 3)].GetComponent<AnimalChara>();
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = upperBody.GetUpperSprite();
        lowBody = animals[Random.Range(0, 3)].GetComponent<AnimalChara>();
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = lowBody.GetLowSprite();
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(attackBoxPosition.position, attackBoxSize);
    }
}
