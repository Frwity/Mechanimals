using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    // USP
   
    [SerializeField] GameObject[] animals;

    [HideInInspector] public AnimalChara upperBodyChara;
    [HideInInspector] public AnimalChara lowBodyChara;

    GameObject upperBody;
    GameObject lowerBody;

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
    public bool isComboUse = false;
    public bool canCombo = false;

    //Animation
    Animator upperBodyAnimator;
    Animator lowerBodyAnimator;

    // Stats
    [SerializeField] public int maxLife = 2;
    public int life;
    public bool isAlive = true;

    int comboCounter = 0;

    //Audio
    [SerializeField] AudioClip[] sounds;

    public AudioSource audioSource;

    public void Awake()
    {
        life = maxLife;
        rb = GetComponent<Rigidbody2D>();
        life = maxLife;
        attackBoxSize.x = upperBodyChara.GetRange();
 		attackBoxPosition.Translate(new Vector3((attackBoxSize.x - 3.5f )/ 2, 0.0f, 0.0f));

        specialBoxPosition.GetComponent<SpecialBox>().Initiate();
        specialBoxPosition.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (!isAlive)
            return;
        invulnerabilityTimer += Time.deltaTime;
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= upperBodyChara.GetAttackSpeed())
            {
                isAttacking = false;
                attackTimer = 0.0f;
                rb.simulated = true;
                rb.velocity = Vector2.zero;
            }
        }

        if(comboCounter > 0)
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

 		if (!isAlive)
            return;      
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
        upperBodyAnimator.SetBool("IsMoving", isMoving);
        lowerBodyAnimator.SetBool("IsMoving", isMoving);
    }

    public void OnJump(CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            jump = context.ReadValue<float>();
            rb.velocity = new Vector2(rb.velocity.x, jump * jumpForce);
            jump = 0f;
            isGrounded = false;
            upperBodyAnimator.SetBool("IsJumping", true);
            upperBodyAnimator.SetBool("IsGrounded", isGrounded);
            lowerBodyAnimator.SetTrigger("Jump");
            lowerBodyAnimator.SetBool("IsGrounded", isGrounded);
            upperBodyAnimator.SetBool("IsMoving", false);
            lowerBodyAnimator.SetBool("IsMoving", false);
            audioSource.PlayOneShot(sounds[0]);
        }
    }

    public void OnAttack(CallbackContext context)
    {
        if(context.performed)
        {
            //TODO polish : when spamming attack button wrong animation is lauch sometimes

            //if combo is finished the player has to wait until he is on the ground
            if (!isGrounded && upperBodyAnimator.GetInteger("NumCombo") == 3)
                return;

            if (!isGrounded)
                rb.simulated = false;

            isAttacking = true;

            if(comboCounter <maxCombo)
            {
                comboCounter++;
                comboTimer = 0.0f;

                if (canCombo && comboCounter < 3)
                    upperBodyAnimator.SetBool("CanCombo", canCombo);
                
                //TODO polish use animation event
                switch(comboCounter)
                {
                    case 1:
                        audioSource.PlayOneShot(sounds[1]);
                        break;
                    case 2:
                        audioSource.PlayOneShot(sounds[2]);
                        break;
                    case 3:
                        audioSource.PlayOneShot(sounds[3]);
                        break;
                    default:
                        break;
                }

                upperBodyAnimator.SetBool("IsAttacking", isAttacking);
                upperBodyAnimator.SetInteger("NumCombo", comboCounter);
                lowerBodyAnimator.SetBool("IsAttacking", isAttacking);
                upperBodyAnimator.SetBool("IsMoving", false);
                lowerBodyAnimator.SetBool("IsMoving", false);
                //TODO Check collision with animation event ?
                CheckAttackCollision();
            }
        }
    }

    public void EndComboAnimation()
    {
        upperBodyAnimator.SetBool("CanCombo", false);
    }

    public void OnDebug(CallbackContext context)
    {
        if (context.started)
        {
            int r = Random.Range(0, animals.Length);
            RandomChangeBody(r, (r + Random.Range(1, animals.Length)) % animals.Length);
            attackBoxSize.x = upperBodyChara.GetRange();
        }
    }

    public void OnSpecial(CallbackContext context)
    {
        if (specialTimer >= lowBodyChara.GetSpecialCoolDown() && !isSpecial && context.started)
        {
            specialTimer = 0f;
            isSpecial = true;
            upperBodyAnimator.SetTrigger("SpecialAttack");
            lowerBodyAnimator.SetTrigger("SpecialAttack");
            audioSource.PlayOneShot(lowBodyChara.animalSounds[0]);
            lowBodyChara.InitiateSpecialAttack(this);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            upperBodyAnimator.SetBool("IsGrounded", isGrounded);
            upperBodyAnimator.SetBool("IsJumping", false);
            lowerBodyAnimator.SetBool("IsGrounded", isGrounded);

            if (upperBodyAnimator.GetInteger("NumCombo") == 3)
                upperBodyAnimator.SetInteger("NumCombo", 0);
        }
    }

    public Vector2 RandomChangeBody(int x, int y) // goat bear crab
    {
        if (upperBody != null)
            Destroy(upperBody);

        if (lowerBody != null)
            Destroy(lowerBody);

        //int lowNo = Random.Range(0, animals.Length);
        //int upperNo = (lowNo + Random.Range(1, animals.Length)) % animals.Length;
        int lowNo = (++x) % animals.Length;
        int upperNo = (++y) % animals.Length;

        //Upper Body setup
        upperBodyChara = animals[upperNo].GetComponent<AnimalChara>();
        upperBody = Instantiate(upperBodyChara.GetUpperSprite(), transform.GetChild(0).position, Quaternion.identity);
        upperBody.transform.parent = transform;
        upperBodyAnimator = upperBody.GetComponent<Animator>();

        //Lower body setup
        lowBodyChara = animals[lowNo].GetComponent<AnimalChara>();
        lowerBody = Instantiate(lowBodyChara.GetLowSprite(), transform.GetChild(1).position, Quaternion.identity);
        lowerBody.transform.parent = transform;
        lowerBodyAnimator = lowerBody.GetComponent<Animator>();

        if (lowNo == 0)
            specialBoxPosition.transform.localScale = new Vector3(1, 1);
        else if (lowNo == 1)
            specialBoxPosition.transform.localScale = new Vector3(lowBodyChara.GetSpecialSize(), 1);

		return new Vector2(lowNo, upperNo);
    }

  
    private void CheckAttackCollision()
    {
        Collider2D[] collideEnemies = Physics2D.OverlapBoxAll(attackBoxPosition.position, attackBoxSize, 0, damageable);

        foreach(Collider2D enemy in collideEnemies)
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
        if (upperBodyAnimator.GetInteger("NumCombo") == 3)
            comboCounter = 0;

        isAttacking = false;
        rb.simulated = true;
        rb.velocity = Vector2.zero;
        upperBodyAnimator.SetBool("IsAttacking", isAttacking);
        lowerBodyAnimator.SetBool("IsAttacking", isAttacking);
    }

    public void TakeDamage(int damage, Vector2 knockbackVelocity) //
    {
        if (invulnerabilityTimer < invulnerabilityTime)
            return;
        invulnerabilityTimer = 0.0f;

        life = Mathf.Clamp(life, 0, life - damage);

        //reset combo when hit
        comboCounter = 0;
        GetComponent<Rigidbody2D>().velocity = knockbackVelocity;

        if (life <= 0)
        {
            isAlive = false;
            Invoke("Die", 1f);
        }

        upperBodyAnimator.SetTrigger("Hit");
        lowerBodyAnimator.SetTrigger("Hit");

        if (life == 0)
            isAlive = false;
    }

	void Die()
    {
        if (!isAlive)
            gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(attackBoxPosition.position, attackBoxSize);
    }
}
