using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
// using TMPro;

public abstract class  Unit : MonoBehaviour, IComparable
{   
    private Vector3 originPos;
    
    public enum InCombat 
    {
        MOVETO,
        MOVEAWAY
    }
    public InCombat inCombat;
    
    public enum UnitTeam 
    {
        PLAYERFACTION,
        ENEMYFACTION,
        NEUTRALFACTION
    }
    protected UnitTeam unitTeam;

    public enum UnitTurnState
    {
        ACTIVE,
        INACTIVE
    }
    UnitTurnState unitTurnState = UnitTurnState.ACTIVE;
    
    public bool GetState()
    {
        if(unitTurnState == UnitTurnState.ACTIVE)
            return true;
        else 
            return false;
    }
    
    public enum UnitStatus
    {
        ALIVE,
        DEAD
    }
    UnitStatus unitStatus = UnitStatus.ALIVE; 

    public bool GetStatus()
    {
        if(unitStatus == UnitStatus.ALIVE)
            return true;
        else 
            return false;
    }
    
    public string UnitName { get; set; }
    [SerializeField]public int Level;        
    [SerializeField]public int Initative;    
    
    [SerializeField]
    public int MaxHealth;
    
    [SerializeField]
    private int currentHealth;

    public int CurrentHealth 
    {
        get
        {
            return currentHealth;
        }

        set
        {
            if (value > MaxHealth)
                currentHealth = MaxHealth;
            
            else if ( value < 0)
                currentHealth = 0;
            
            else
                currentHealth = value;
            
        }
    }

    // This is used to compare initatives when sorting the unit list in CombatSystems script
    public int CompareTo(object otherObj)
    {
        var a = this;
        var b = otherObj as Unit;

        if (a.Initative < b.Initative)
            return 1;

        if (a.Initative > b.Initative)
            return -1;

        return 0;
        
        // return Initative.CompareTo(((Unit)otherObj).Initative);
    }

    // public int CompareTo(object unit1, object unit2)
    // {
    //     return ((Unit)unit1).Initative.CompareTo(((Unit)unit2).Initative);
    // }


    public FillBar healthBar;
    [SerializeField] private Transform pfDamagePopUp;
    [SerializeField] private Transform unitGO;

    protected virtual void Start()
    {
        CurrentHealth = MaxHealth;
        healthBar.SetMaxValue(MaxHealth);
        inCombat = InCombat.MOVEAWAY;
        originPos = transform.position;
    }

    protected virtual void Update()
    {
       healthBar.SetCurrentValue(CurrentHealth);

        if (this.inCombat == InCombat.MOVETO)
            this.MoveToCombat();

        if (this.inCombat == InCombat.MOVEAWAY)
            this.MoveFromCombat();

    }
    

    // This is for dealing the damage to target unit
    // Gets called from the attacking unit
    // Sets the damage for the DamagePopUp script and calls the script to instantiate the damage pop up  
    public void TakeDamage ( int damage, string type )
    {
        CurrentHealth -= damage;
        Debug.Log("Damage taken: " + damage);

        HandleDamagePU( damage, type );
        // Transform damagePopUpTransform = Instantiate(pfDamagePopUp, unitGO);
        // DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        // damagePopUp.HandePopUp(damage);

        if ( currentHealth <= 0)
        {
            unitStatus = UnitStatus.DEAD;
            Debug.Log("This unit is dead 1");
            this.StartCoroutine(HandleDeath());
            Debug.Log("This unit is dead 2");
        }
        else 
            unitStatus = UnitStatus.ALIVE;
    }

    // Similar to TakeDamage, gets called from an ability
    public void HealDamage ( int damageHeal)
    {
        CurrentHealth += damageHeal;
        Debug.Log("Damage healed: " +damageHeal);
        string type = "heal";
        HandleDamagePU( damageHeal, type );
    }

    // The actual enemy attack
    // Randomizes the damage and then calls the TakeDamage of the target unit
    // Calls the NextTurn from CombatSystem to advance to the next unit
    // Overridden from specialized unit classes
    public virtual IEnumerator EnemyAction(GameObject targetUnit, Action NextTurn) 
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Enemy starting attack"); 
        Unit target = targetUnit.GetComponent<Unit>();
        int dmg = Random.Range(15, 30);
        target.TakeDamage(dmg, "normal");

        Debug.Log("Enemy done, switching to next unit");
        // this.unitTurnState = UnitTurnState.INACTIVE;
        yield return new WaitForSeconds(1.0f);
        
        NextTurn();
    }

    // Same as the enemy attack, but for player
    public virtual IEnumerator PlayerAction(GameObject targetUnit, Action NextTurn) 
    {
        Debug.Log("Player starting attack");
        yield return new WaitForSeconds(1.0f);
        //Attack logic here 
        Unit target = targetUnit.GetComponent<Unit>();
        
        this.inCombat = InCombat.MOVETO;
        target.inCombat = InCombat.MOVETO;
        
        int dmg = Random.Range(45, 61);
        target.TakeDamage(dmg, "normal");
        
        yield return new WaitForSeconds(1.0f);

        this.inCombat = InCombat.MOVEAWAY;
        target.inCombat = InCombat.MOVEAWAY;

        Debug.Log("Player done, switching to next unit");
        yield return new WaitForSeconds(1.0f);
        NextTurn();
        // this.unitTurnState = UnitTurnState.INACTIVE;
    }

    public void MoveToCombat()
    {
        var pos = transform.position;
        if (this.unitTeam == UnitTeam.PLAYERFACTION)
        {
            pos.x = -2;
            transform.position = pos;
        }
        else
        {
            pos.x = 2;
            transform.position = pos;
        }
    }

    public void MoveFromCombat()
    {
        var pos = originPos;
        pos.x = originPos.x;
        transform.position = pos;
    }

    public void HandleDamagePU(int damage, string type)
    {
        Transform damagePopUpTransform = Instantiate(pfDamagePopUp, unitGO);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.HandePopUp(damage, type);
    }

    private IEnumerator HandleDeath()
    {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("Unit is dead, moving up");

            var pos = transform.position;
            pos.y = 15;
            transform.position = pos;

            // Destroy(gameObject, 2.0f);
    }

}
