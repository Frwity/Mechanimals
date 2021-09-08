using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    int damage = 1;
    [SerializeField] float speed = 5f;
    GameObject target;
    [SerializeField] float upTime = 1.5f;
    float upTimer;
    bool isPicking = false;

    public void Update()
    {
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
            collision.GetComponent<Enemy>().TakeDamage(damage, 3, Vector2.one);
            Destroy(gameObject);
        }
    }

    public void InitiateMissile(int _damage, float _speed, GameObject _target)
    {
        damage = _damage;
        speed = _speed;
        target = _target;
    }
}
