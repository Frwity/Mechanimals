using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBox : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && transform.parent.CompareTag("Player"))
            collision.GetComponent<Enemy>().TakeDamage(transform.parent.GetComponent<Player>().upperBodyChara.GetSpecialDamage(), 3, Vector2.one);
    }
}
