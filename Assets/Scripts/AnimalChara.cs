using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalChara : MonoBehaviour
{
    [SerializeField] Sprite upperSprite;
    [SerializeField] Sprite lowSprite;

    [SerializeField] protected float range = 1f;
    [SerializeField] protected float attackSpeed = 0.5f;
    [SerializeField] protected float knockbackForce = 40f;
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float specialCoolDown = 3f;
    [SerializeField] protected int specialDamage = 1;
    [SerializeField] protected float specialSize = 4f;
    [SerializeField] protected GameObject missilePrefab;


    public Sprite GetUpperSprite() { return upperSprite; }
    public Sprite GetLowSprite() { return lowSprite; }

    public float GetRange() { return range; }
    public float GetAttackSpeed() { return attackSpeed; }
    public float GetKnockbackForce() { return knockbackForce; }
    public float GetSpeed() { return speed; }
    public float GetSpecialCoolDown() { return specialCoolDown; }
    public int GetSpecialDamage() { return specialDamage; }
    public float GetSpecialSize() { return specialSize; }

    public virtual bool PerformSpecialAttack(Player player)
    {
        return false;
    }
    public virtual void InitiateSpecialAttack(Player player)
    {
    }
}
