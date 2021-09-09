using UnityEngine;

public class AnimationCancel : MonoBehaviour
{
    [SerializeField] Player p;

    private void Start()
    {
        p = transform.parent.GetComponent<Player>();
    }

    public void EndAttack()
    {
        p.EndAttack();
    }
}
