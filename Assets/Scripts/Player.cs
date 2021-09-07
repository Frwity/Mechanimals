using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    // Movement

    private Rigidbody2D rb;
    [SerializeField] float jumpForce = 100f;
    [SerializeField] float speed = 3f;

    //Combat
    [SerializeField] AttackBox attackBox;
    [SerializeField] float attackCooldown = 1.0f;
    float attackTimer = 0.0f;

    bool hasAttack = false;

    Vector2 moveInput;
    float jump = 1f;
    bool isGrounded = true;
    bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(hasAttack)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackCooldown)
            {
                hasAttack = false;
                attackTimer = 0.0f;
            }    
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            transform.Translate((moveInput.x < 0f ? Mathf.Floor(moveInput.x) : Mathf.Ceil(moveInput.x)) * Time.deltaTime * speed, 0f, 0f);
        }
    }

    public void OnMovement(CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = !context.canceled;
    }

    public void OnJump(CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            jump = context.ReadValue<float>();
            rb.velocity = new Vector2(rb.velocity.x, jump * jumpForce);
            jump = 0f;
            isGrounded = false;
        }
    }

    public void OnAttack(CallbackContext context)
    {
        if(context.performed && !hasAttack)
        {
            hasAttack = true;
            attackBox.Attack();
        }
    }

    public void OnDash(CallbackContext context)
    {
    }
    public void OnSpecial(CallbackContext context)
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            isGrounded = true;
    }
}
