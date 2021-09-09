using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.collider.GetComponent<Player>().TakeDamage(100000, Vector2.zero);
        if (collision.collider.CompareTag("Enemy"))
            collision.collider.GetComponent<Enemy>().TakeDamage(100000, 3, Vector2.zero);
    }
}
