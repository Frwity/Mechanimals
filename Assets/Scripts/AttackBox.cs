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

    bool hasHit = true;

    bool hasAttack = false;

    int comboCounter = 0;

    int attackCounter = 0;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
    }

    private void Update()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Enemy en = collision.gameObject.GetComponent<Enemy>();
            en.takeDamage(1, comboCounter);
            comboCounter++;
            Debug.Log("Combo : " + comboCounter);
        };
    }
}
