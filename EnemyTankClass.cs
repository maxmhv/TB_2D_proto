using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
// using TMPro;

public class EnemyTankClass : Unit

{
    private void Awake() 
    {
        unitTeam = UnitTeam.ENEMYFACTION;
        UnitName = "Enemy Tank";
        Level = 3;
        Initative = 4;
        MaxHealth = Random.Range(300, 350);   
    }
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override IEnumerator EnemyAction(GameObject targetUnit, Action NextTurn) 
    {
        Debug.Log("Enemy choosing action");
        yield return new WaitForSeconds(0.5f);
        
        Unit target = targetUnit.GetComponent<Unit>();
        int dmg;

        this.inCombat = InCombat.MOVETO;
        target.inCombat = InCombat.MOVETO;
        
        float action = Random.value;
        if (action < .8f) // Normal attack
        {
            Debug.Log("Enemy uses normal attack");
            dmg = Random.Range(15, 20);
            // target.TakeDamage(dmg, "normal");

        }
        else // Special attack
        {
            Debug.Log("Enemy uses heavy strike");
            dmg = 40;
            // target.TakeDamage(dmg);
        }
        target.TakeDamage(dmg, "normal");

        // Debug.Log("Enemy starting attack"); 
        // Unit target = targetUnit.GetComponent<Unit>();
        // int dmg = Random.Range(15, 30);
        // target.TakeDamage(dmg);

        Debug.Log("Enemy done, switching to next unit");
        // // this.unitTurnState = UnitTurnState.INACTIVE;
        yield return new WaitForSeconds(1.0f);

        this.inCombat = InCombat.MOVEAWAY;
        target.inCombat = InCombat.MOVEAWAY;
        
        NextTurn();
    }
}
