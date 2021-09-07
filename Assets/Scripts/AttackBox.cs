using UnityEngine;

public class AttackBox : MonoBehaviour
{
    [SerializeField] float attackWindow = 0.5f;
    float attackTimer = 0.0f;

    [SerializeField] float ComboUptime = 2.0f;
    float comboTimer = 0.0f;

    [SerializeField] int maxCombo = 3;

    BoxCollider2D boxCollider;
    SpriteRenderer sp;

    bool hasAttack = false;

    int comboCounter = 0;

    public void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if(gameObject.activeSelf)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackWindow)
            {
                FinishAttack();
            }
        }

        //Combo
        if(hasAttack)
        {
            comboTimer += Time.deltaTime;
            if(comboTimer >= ComboUptime || comboCounter == maxCombo)
            {
                comboCounter = 0;
                hasAttack = false;
                comboTimer = 0.0f;
            }
        }
    }

    public void Attack() 
    {
        hasAttack = true;
        boxCollider.enabled = true;
        sp.enabled = true;
        attackTimer = 0.0f;
    }

    public void FinishAttack()
    {
        boxCollider.enabled = false;
        sp.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && transform.parent.tag == "Player")
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(1, comboCounter);
            comboCounter++;
            Debug.Log("Combo : " + comboCounter);
        }

        if (collision.gameObject.tag == "Player" && transform.parent.tag == "Enemy")
        {
            comboCounter++;
            collision.gameObject.GetComponent<Player>().TakeDamage(1);
        }
    }
}
