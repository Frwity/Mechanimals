using UnityEngine;

public class AttackBox : MonoBehaviour
{

    [SerializeField] float attackWindow = 0.5f;
    float attackTimer = 0.0f;

    bool hasHit = true;

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
    }

    public void Attack()
    {
        gameObject.SetActive(true);
        attackTimer = 0.0f;
    }

    public void FinishAttack()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Enemy en = collision.gameObject.GetComponent<Enemy>();
            en.takeDamage(1);
        };
    }
}
