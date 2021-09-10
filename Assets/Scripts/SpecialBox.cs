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
            player.audioSource.PlayOneShot(player.lowBodyChara.animalSounds[1]);

            if(player.lowBodyChara.specialImpactParticle)
                GameObject.Instantiate(player.lowBodyChara.specialImpactParticle, enemy.transform.position, Quaternion.identity);
            

            enemy.TakeDamage(player.lowBodyChara.GetSpecialDamage(), 3,
                (transform.position.x < enemy.transform.position.x ? new Vector3(player.knockBackDirection.x, player.knockBackDirection.y) 
                : new Vector3(-player.knockBackDirection.x, player.knockBackDirection.y)) * player.lowBodyChara.GetSpecialKnockbackForce());
        }
    }
}
