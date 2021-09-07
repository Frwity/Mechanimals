using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalChara : MonoBehaviour
{
    [SerializeField] Sprite upperSprite;
    [SerializeField] Sprite lowSprite;

    [SerializeField] float range = 1f;
    [SerializeField] float attackSpeed = 0.5f;
    [SerializeField] float knockbackForce = 40f;
    [SerializeField] float speed = 3f;

    public Sprite GetUpperSprite() { return upperSprite; }
    public Sprite GetLowSprite() { return lowSprite; }

    public float GetRange() { return range; }
    public float GetAttackSpeed() { return attackSpeed; }
    public float GetKnockbackForce() { return knockbackForce; }
    public float GetSpeed() { return speed; }

    public virtual bool PerformSpecialAttack(Player player)
    {
        return false;
    }
    public virtual void InitiateSpecialAttack(Player player)
    {
    }
}
