using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class CombatSystem : MonoBehaviour
{
    public enum BattleState { BUSY, WAITING, READY, START, WON, LOST }
    public BattleState bState;

    private List<Unit> unitList;
    private List<Unit> enemyList;
    private List<Unit> enemyDeadList;
    
    private int RoundCount;

    public GameObject playerPrefab;
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;

    public GameObject playerGO;
    public GameObject enemyGO;
    public GameObject enemyGO2;

    public Transform playerStation;
    public Transform enemyStation1;
    public Transform enemyStation2;

    public Unit playerUnit;
    public Unit enemyUnit;
    public Unit enemyUnit2;

    public TMP_Text HUDText;
    public GameObject playerTarget;

    // public CombatHUD playerHUD;
    // public CombatHUD enemyHUD;

    void Start()
    {
        Debug.Log("New combat starting");
        HUDText.text = "Combat start!";
        RoundCount = 0;
        this.SetupCombat(); // Spawns units to the map 
        bState = BattleState.BUSY;

        //Add all units to the unit list
        unitList = new List<Unit>();
        enemyList = new List<Unit>();
        enemyDeadList = new List<Unit>();

        GameObject[] playerUnitsList = GameObject.FindGameObjectsWithTag("PlayerUnit");

        foreach (GameObject playerUnit in playerUnitsList)
        {
            Unit currentUnit = playerUnit.GetComponent<Unit>();
            unitList.Add(currentUnit);   
        }

        GameObject[] enemyUnitsList = GameObject.FindGameObjectsWithTag("EnemyUnit");

        foreach (GameObject enemyUnit in enemyUnitsList)
        {
            Unit currentUnit = enemyUnit.GetComponent<Unit>();
            unitList.Add(currentUnit);
            enemyList.Add(currentUnit);   
        }
        playerTarget = enemyGO;
        this.NextRound();    

    }

    // Spawn the units to the map and set up the references for later use 
    void SetupCombat()
    {

        playerGO = Instantiate(playerPrefab, playerStation);
        playerUnit = playerGO.GetComponent<Unit>();

        int rndEnemy = Random.Range(1,3);
        switch (rndEnemy)
        {
            case 1:
            {
                enemyGO = Instantiate(enemyPrefab1, enemyStation1);
                enemyUnit = enemyGO.GetComponent<Unit>();
                break;
            }
            case 2: // Can use enemyGO(1) to instantiate this one too
            {
                enemyGO = Instantiate(enemyPrefab2, enemyStation1);
                enemyUnit = enemyGO.GetComponent<Unit>();
                break;
            }
        }
        int rndEnemy2 = Random.Range(1,3);
        switch (rndEnemy2)
        {
            case 1:
            {
                enemyGO2 = Instantiate(enemyPrefab1, enemyStation2);
                enemyUnit2 = enemyGO2.GetComponent<Unit>();
                break;
            }
            case 2:
            {
                enemyGO2 = Instantiate(enemyPrefab2, enemyStation2);
                enemyUnit2 = enemyGO2.GetComponent<Unit>();
                break;
            }
        }

        // playerHUD.SetupHUD(playerUnit);
        // enemyHUD.SetupHUD(enemyUnit);

    }

    // Gets the first unit from the unit list, then removes and adds it back to the list without sorting
    public void NextTurn()
    {
            bState = BattleState.BUSY;
            Unit currentUnit = unitList[0];
            unitList.Remove(currentUnit);

            // If current unit is both ALIVE and ACTIVE
            if(currentUnit.GetStatus()&&currentUnit.GetState()&&enemyDeadList.Count < enemyList.Count)
            {
                unitList.Add(currentUnit);

                // If the current unit is player, stops to wait for player input
                // If the unit is enemy, activates the enemy's action 
                if(currentUnit.tag == "PlayerUnit")
                {
                    Debug.Log("Player turn");
                    HUDText.text = "Player turn";
                    bState = BattleState.WAITING;
                }
                else
                {
                    Debug.Log("Enemy turn");
                    HUDText.text = "Enemy turn";
                    StartCoroutine(HandleTurn(currentUnit, playerGO));
                }
            }
            // If current unit is DEAD
            // Sets the state depending if the unit is player or enemy
            else if (currentUnit.GetStatus() == false) 
            {
                if (currentUnit.tag == "EnemyUnit" && enemyDeadList.Count < enemyList.Count)
                    {   
                        enemyDeadList.Add(currentUnit);
                        Debug.Log("enemyDeadList" + enemyDeadList.Count);
                        Debug.Log("enemyList" + enemyList.Count);
                        // bState = BattleState.WON;  
                        if (enemyDeadList.Count < enemyList.Count)
                            this.NextTurn();
                        else
                        {
                            bState = BattleState.WON; 
                            BattleEnd();
                        }
                    }
                else 
                {
                    if (currentUnit.tag == "PlayerUnit")
                        bState = BattleState.LOST;
                    else if (currentUnit.tag == "EnemyUnit")
                        bState = BattleState.WON;  
                    
                    BattleEnd();
                }
            }
            else this.NextTurn(); // If current unit is ALIVE && INACTIVE
            //Change to NextRound
    }
    
    // Gets the current enemy unit and the player unit as the target
    // Uses the enemy's Unit script to deal damage to the player
    IEnumerator HandleTurn(Unit currentUnit, GameObject targetUnit) 
    {
        Debug.Log("Enemy unit is acting");
        yield return new WaitForSeconds(1.0f);
        HUDText.text = "Enemy attacks!";

        StartCoroutine(currentUnit.EnemyAction(targetUnit, NextTurn));

    }

    // Advances the round count and sorts the unit list by initative
    // Highest initative goes first
    public void NextRound()
    {
        Debug.Log("New round is starting");
        RoundCount = RoundCount + 1;
        Debug.Log("Current round: "+ RoundCount);

        // cycle through units and set ACTIVE
        // Create list in here, call from start? 

        unitList.Sort();
        
        this.NextTurn();

    }

    public void BattleEnd()
    {
        if (bState == BattleState.WON)
            {
            Debug.Log("Player has won");
            HUDText.text = "Player wins!";
            }
        else
        {
            Debug.Log("Enemy has won");
            HUDText.text = "Enemy wins!";
            }
    }

    // As long as it is the player's turn, the game is WAITING for player input
    // --> On space, activate player's attack and advance to next unit't turn
    void Update() 
    {
        if ( bState == BattleState.WAITING )
        {
            if (Input.GetKeyDown("m")) // For testing
            {
                NextTurn();
            }    
            if (Input.GetKeyDown("n")) // For testing
            {
                NextRound();
            }

            if (enemyGO.GetComponent<Unit>().GetStatus() == false && playerTarget == enemyGO )
            {
                playerTarget = enemyGO2;
            }
            
            if (enemyGO2.GetComponent<Unit>().GetStatus() == false && playerTarget == enemyGO2 )
            {
                playerTarget = enemyGO;
            }

            if (Input.GetKeyDown("1")) // Set target for players attack, default is 1st enemy
            {
                if (enemyGO.GetComponent<Unit>().GetStatus())
                {
                    playerTarget = enemyGO;
                    Debug.Log("Target changed to 1st enemy");
                }
                else 
                {
                    playerTarget = enemyGO2;
                    Debug.Log("Target changed to 2nd enemy");
                }
            }
            if (Input.GetKeyDown("2")) // Same as above
            {
                if (enemyGO2.GetComponent<Unit>().GetStatus())
                {
                    playerTarget = enemyGO2;
                    Debug.Log("Target changed to 2nd enemy");
                }
                else 
                {
                    playerTarget = enemyGO;
                    Debug.Log("Target changed to 1st enemy");
                }
            }
            if (Input.GetKeyDown("space"))
            {
                HUDText.text = "Player attacks!";
                bState = BattleState.BUSY;
                StartCoroutine(playerUnit.PlayerAction(playerTarget, NextTurn)); // Target from click
            }
        }
    }

}
