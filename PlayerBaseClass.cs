using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerBaseClass : Unit
{

    private void Awake() 
    {
        unitTeam = UnitTeam.PLAYERFACTION;
        UnitName = "Player Soldier";
        Level = 2;
        Initative = 8;
        MaxHealth = 250;
    } 

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {

        if (Input.GetKeyDown("4"))
        {
            TakeDamage(50, "normal");
        }
        if (Input.GetKeyDown("3"))
        {
            HealDamage(30);
        }

        base.Update();
        
    }

    public override IEnumerator PlayerAction(GameObject targetUnit, Action NextTurn)
    {
        Debug.Log("Player starting attack");
        yield return new WaitForSeconds(1.0f);
        Unit target = targetUnit.GetComponent<Unit>();
        int dmg;
        
        this.inCombat = InCombat.MOVETO;
        target.inCombat = InCombat.MOVETO;
        
        float crit = Random.value;
        if (crit < .8f) // Normal damage
        {
            Debug.Log("Player normal attack");
            dmg = Random.Range(40, 61);
            target.TakeDamage(dmg, "normal");
        }

        else // Critical damage
        {
            Debug.Log("Player crits");
            dmg = 120;
            target.TakeDamage(dmg, "crit");
        }

        // int dmg = Random.Range(45, 61);
        // target.TakeDamage(dmg);
        
        yield return new WaitForSeconds(1.0f);

        this.inCombat = InCombat.MOVEAWAY;
        target.inCombat = InCombat.MOVEAWAY;

        Debug.Log("Player done, switching to next unit");
        yield return new WaitForSeconds(1.0f);
        NextTurn();
        // this.unitTurnState = UnitTurnState.INACTIVE;
    }
}
