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
        p.canCombo = false;
    }

    public void ComboCanBePressed()
    {
        p.EndComboAnimation();
        p.canCombo = true;
    }

    public void EndComboCanBePressed()
    {
        p.canCombo = false;
    }
}
