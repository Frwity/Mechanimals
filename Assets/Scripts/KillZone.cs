using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.collider.GetComponent<Player>().TakeDamage(collision.collider.GetComponent<Player>().life, Vector2.zero);
        if (collision.collider.CompareTag("Enemy"))
            collision.collider.GetComponent<Enemy>().TakeDamage(collision.collider.GetComponent<Enemy>().life, 3, Vector2.zero);
    }
}
