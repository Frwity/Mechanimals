using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    int damage = 1;
    [SerializeField] float speed = 5f;
    GameObject target;
    Vector2 knockback;
    [SerializeField] float upTime = 1.5f;
    float upTimer;
    bool isPicking = false;

    public void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        if (upTimer >= upTime)
            isPicking = true;
        else
            upTimer += Time.deltaTime;

        if (isPicking)
            transform.Translate((target.transform.position - transform.position).normalized * speed * Time.deltaTime);
        else
            transform.Translate(Vector3.up * speed / 1.5f * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(damage, 3, transform.position.x < collision.transform.position.x ? knockback : new Vector2(-knockback.x, knockback.y));
            Destroy(gameObject);
        }
    }

    public void InitiateMissile(int _damage, float _speed, GameObject _target, Vector2 _knockback)
    {
        damage = _damage;
        speed = _speed;
        target = _target;
        knockback = _knockback;
    }
}
