using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBox : MonoBehaviour
{
    List<GameObject> hitEnemy;

    public void ReInitiate()
    {
        hitEnemy.Clear();
    }

    public void Initiate()
    {
        hitEnemy = new List<GameObject>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && transform.parent.CompareTag("Player"))
        {
            foreach (GameObject go in hitEnemy)
            {
                if (go == collision.gameObject)
                    return;
            }
            hitEnemy.Add(collision.gameObject);

            Enemy enemy = collision.GetComponent<Enemy>();
            Player player = transform.parent.GetComponent<Player>();
            enemy.TakeDamage(player.upperBodyChara.GetSpecialDamage(), 3,
                (transform.position.x < enemy.transform.position.x ? player.knockBackDirection : new Vector2(-player.knockBackDirection.x, player.knockBackDirection.y)) * player.upperBodyChara.GetKnockbackForce());
        }
    }
}