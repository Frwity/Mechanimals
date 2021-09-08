using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    int damage = 1;
    float speed = 3;
    Vector2 direction;

    float timer = 0;

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 10f)
            Destroy(gameObject);

        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Ground"))
            Destroy(gameObject);
    }

    public void InitiateMissile(int _damage, float _speed, Vector2 _direction)
    {
        damage = _damage;
        speed = _speed;
        direction = _direction;
    }
}
