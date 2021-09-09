using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeTarget : Enemy
{
    public float timer = 0f;
    public bool isHit = false;
    public new SpriteRenderer renderer;

    public override void Start()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public override void Update()
    {
        if (isHit)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                isHit = false;
                timer = 0f;
            }
            renderer.color = Color.red;
        }
        else
            renderer.color = Color.green;
    }

    public override void TakeDamage(int damage, int comboNum, Vector3 knockBackVelocity)
    {
        isHit = true;
    }
}
