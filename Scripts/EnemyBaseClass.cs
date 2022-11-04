using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseClass : Unit
{

     
    protected override void Start()
    {
        unitTeam = UnitTeam.ENEMYFACTION;
        UnitName = "Enemy Warrior";
        Level = 2;
        Initative = 3;
        MaxHealth = Random.Range(200, 350);

        base.Start();
    }

    protected override void Update()
    {
        base.Update();   
    }
}