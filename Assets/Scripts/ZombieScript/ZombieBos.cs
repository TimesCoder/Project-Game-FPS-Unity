using UnityEngine;

public class ZombieBos : ZombieBiasa
{
    protected override void Start()
    {
        maxHealth = 250;
        attackDamage = 15; // Damage lebih besar dari zombie biasa
        base.Start();
    }
}
