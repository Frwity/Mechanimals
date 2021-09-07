using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabChara : AnimalChara
{
    public override bool PerformSpecialAttack(Player player)
    {
        return false;
    }
    public override void InitiateSpecialAttack(Player player)
    {
    }
}
