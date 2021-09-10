using UnityEngine;

public class AnimalChara : MonoBehaviour
{
    //[SerializeField] Sprite upperSprite;
    //[SerializeField] Sprite lowSprite;

    [SerializeField] GameObject upperSprite;
    [SerializeField] GameObject lowSprite;

    [SerializeField] protected float range = 1f;
    [SerializeField] protected float attackSpeed = 0.5f;
    [SerializeField] protected float knockbackForce = 40f;
    [SerializeField] protected float specialKnockbackForce = 40f;
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float specialCoolDown = 3f;
    [SerializeField] protected int specialDamage = 1;
    [SerializeField] protected float specialSize = 4f;
    [SerializeField] protected GameObject missilePrefab;
    [SerializeField] public AudioClip[] animalSounds;
    public GameObject impact1Particle;
    public GameObject impact3Particle;
    public GameObject specialImpactParticle;


    public GameObject GetUpperSprite() { return upperSprite; }
    public GameObject GetLowSprite() { return lowSprite; }

    public float GetRange() { return range; }
    public float GetAttackSpeed() { return attackSpeed; }
    public float GetKnockbackForce() { return knockbackForce; }
    public float GetSpecialKnockbackForce() { return specialKnockbackForce; }
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
