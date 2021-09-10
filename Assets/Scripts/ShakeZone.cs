using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeZone : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && collision.collider.GetComponent<Enemy>().isHit)
            StartCoroutine(WorldManager.worldManager.camera.transform.GetChild(0).GetComponent<ShakeScreen>().Shake(0.15f, 0.2f));
    }
}
