using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    // USP

    [SerializeField] GameObject[] animals;

    [HideInInspector] public AnimalChara upperBodyChara;
    [HideInInspector] public AnimalChara lowBodyChara;

    // Movement

    [HideInInspector] public Rigidbody2D rb;
    Vector2 moveInput;
    [SerializeField] float jumpForce = 100f;
    float jump = 1f;
    [HideInInspector] public bool isGrounded = true;
    bool isMoving = false;
    [HideInInspector] public bool isFlipped = false;

    // Combat
    [SerializeField] public GameObject specialBoxPosition;

    [SerializeField] Transform attackBoxPosition;
    [SerializeField] Vector2 attackBoxSize;
    [SerializeField] LayerMask damageable;
    [SerializeField] int damage = 1;
    [SerializeField] int maxCombo = 3;
    [SerializeField] float comboUptime = 2.0f;
    [SerializeField] public Vector2 knockBackDirection;
    [SerializeField] float attractionForce = 2.0f;
    [SerializeField] float invulnerabilityTime = 1.0f;

    float invulnerabilityTimer = 0.0f;
    float specialTimer = 0.0f;
    float attackTimer = 0.0f;
    float comboTimer = 0.0f;
    bool isAttacking = false;
    bool isSpecial = false;

    //Animation
    Animator anim;

    // Stats
    [SerializeField] int maxLife = 2;
    int life;
    [HideInInspector] public bool isAlive = true;

    int comboCounter = 0;

    public void Awake()
    {
        life = maxLife;
        rb = GetComponent<Rigidbody2D>();
        RandomChangeBody();
        life = maxLife;
        anim = GetComponent<Animator>();
        attackBoxSize.x = upperBodyChara.GetRange();
        attackBoxPosition.Translate(new Vector3((attackBoxSize.x - 3.5f) / 2, 0.0f, 0.0f));

        specialBoxPosition.GetComponent<SpecialBox>().Initiate();
        specialBoxPosition.SetActive(false);
    }

    public void Update()
    {
        invulnerabilityTimer += Time.deltaTime;
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= upperBodyChara.GetAttackSpeed())
            {
                isAttacking = false;
                attackTimer = 0.0f;
                rb.simulated = true;
                rb.velocity = Vector2.zero;
            }
        }

        if (comboCounter > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= comboUptime)
            {
                comboCounter = 0;
                comboTimer = 0.0f;
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

            if (!isAttacking)
                transform.Translate((moveInput.x > 0 ? moveInput.x : -moveInput.x) * Time.deltaTime * lowBodyChara.GetSpeed(), 0f, 0f);
        }

        specialBoxPosition.SetActive(false);

        if (isSpecial)
            isSpecial = lowBodyChara.PerformSpecialAttack(this);

        specialTimer += Time.deltaTime;
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
        if (context.performed)
        {
            //TODO polish : when spamming attack button wrong animation is lauch sometimes

            //if combo is finished the player has to wait until he is on the ground
            if (!isGrounded && anim.GetInteger("numCombo") == 3)
                return;

            if (!isGrounded)
                rb.simulated = false;

            isAttacking = true;

            if (comboCounter < maxCombo)
            {
                comboCounter++;
                comboTimer = 0.0f;
                anim.SetBool("isAttacking", isAttacking);
                anim.SetInteger("numCombo", comboCounter);
                //TODO Check collision with animation event ?
                CheckAttackCollision();
            }
        }
    }

    public void OnDebug(CallbackContext context)
    {
        if (context.started)
        {
            RandomChangeBody();
            attackBoxSize.x = upperBodyChara.GetRange();
        }
    }

    public void OnSpecial(CallbackContext context)
    {
        if (specialTimer >= lowBodyChara.GetSpecialCoolDown() && !isSpecial && context.started)
        {
            specialTimer = 0f;
            isSpecial = true;
            lowBodyChara.InitiateSpecialAttack(this);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;

            if (anim.GetInteger("numCombo") == 3)
                anim.SetInteger("numCombo", 0);
        }
    }

    private void RandomChangeBody() // goat bear crab
    {
        int upperNo = Random.Range(0, animals.Length);
        int lowNo = (upperNo + Random.Range(1, animals.Length)) % animals.Length;

        upperBodyChara = animals[upperNo].GetComponent<AnimalChara>();
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = upperBodyChara.GetUpperSprite();
        lowBodyChara = animals[lowNo].GetComponent<AnimalChara>();
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = lowBodyChara.GetLowSprite();

        if (lowNo == 0)
            specialBoxPosition.transform.localScale = new Vector3(1, 1);
        else if (lowNo == 1)
            specialBoxPosition.transform.localScale = new Vector3(lowBodyChara.GetSpecialSize(), 1);
    }

    private void CheckAttackCollision()
    {
        Collider2D[] collideEnemies = Physics2D.OverlapBoxAll(attackBoxPosition.position, attackBoxSize, 0, damageable);

        foreach (Collider2D enemy in collideEnemies)
        {
            Enemy en = enemy.gameObject.GetComponent<Enemy>();

            if (en)
            {
                Vector2 knockbackVelocity = Vector2.zero;
                //Knock back enemy
                if (comboCounter == 3)
                    knockbackVelocity = (transform.position.x < en.transform.position.x ? knockBackDirection : new Vector2(-knockBackDirection.x, knockBackDirection.y)) * upperBodyChara.GetKnockbackForce();
                else
                    knockbackVelocity = (attackBoxPosition.position - en.transform.position) * attractionForce;

                en.TakeDamage(damage, comboCounter, knockbackVelocity);

            }
        }
    }

    public void EndAttack()
    {
        if (anim.GetInteger("numCombo") == 3)
            comboCounter = 0;

        isAttacking = false;
        rb.simulated = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("isAttacking", isAttacking);
    }

    public void TakeDamage(int damage, Vector2 knockbackVelocity)
    {
        if (invulnerabilityTimer < invulnerabilityTime)
            return;
        invulnerabilityTimer = 0.0f;

        life = Mathf.Clamp(life, 0, life - damage);

        //reset combo when hit
        comboCounter = 0;

        if (life == 0)
            isAlive = false;
        else
            GetComponent<Rigidbody2D>().velocity = knockbackVelocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(attackBoxPosition.position, attackBoxSize);
    }
}
