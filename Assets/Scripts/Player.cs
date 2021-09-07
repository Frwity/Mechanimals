using System.Collections;
using System.Collections.Generic;
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
    Vector2 moveInput;
    float jump = 1f;
    public bool isGrounded = true;
    bool isMoving = false;
    bool isSpecial = false;
    public bool isFlipped = false;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        RandomChangeBody();
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
        upperBody = animals[Random.Range(0, 2)].GetComponent<AnimalChara>();
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = upperBody.GetUpperSprite();
        lowBody = animals[Random.Range(0, 2)].GetComponent<AnimalChara>();
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = lowBody.GetLowSprite();
    }
}
