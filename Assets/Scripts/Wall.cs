using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //bounce back enemy
           /* Enemy en = collision.gameObject.GetComponent<Enemy>();

            if(en)
            {
                Rigidbody2D rb = en.GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }*/
        }
    }
}
