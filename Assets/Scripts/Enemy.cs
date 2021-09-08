using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int life = 10;
    [SerializeField] float attackCooldown = 1.0f;
    [SerializeField] int damage = 1;
    [SerializeField] Transform attackBoxPosition = null;
    [SerializeField] Vector2 attackBoxSize;
    [SerializeField] LayerMask damageable;

    bool isAlive = true;

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
