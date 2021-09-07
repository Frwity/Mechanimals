using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int life = 10;

    bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int damage)
    {
        Debug.Log("OUCH");
        Mathf.Clamp(life, 0, life - damage);
        if (life == 0)
            isAlive = false;
    }
}
