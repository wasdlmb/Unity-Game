using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class game_controller : MonoBehaviour
{

    //the player's score
    public int gameScore;
    int numberOfBattles = 0;
    public int maximumBattles;
    public float winnerAdvantage;
    public float battleRetardation;
    public float econPower = .2f;
    public Text statusText;

    //creates a bunch of territories
    public Territory oneistan = new Territory(500, 1000, 0, 1000, "Oneistan");
    Territory twoistan = new Territory(200, 450, 0, 1000, "Twoistan");
    Territory threeistan = new Territory(1000, 1500, 0, 1000, "Threeistan");
    Territory fouristan = new Territory(500, 1500, 1, 500, "Fouristan");
    Territory fiveistan = new Territory(500, 2000, 1, 1000, "Fiveistan");

    //class of the territories sides fight over
    public class Territory{

        //value: how much your side wants it(ethnic history, religion, etc.)
        //econ: how much the teritory can produce
        //ownership: 0 is player, 1 is enemy, maybe 2 for independent
        //score: the sum total of these, used for calculating game score
        public int value, econ, ownership, score, strength;
        public string name;
        public Territory inBattleWith;
        public bool inBattle;
        public bool underAttack;

        //constructor
        public Territory(int value, int econ, int ownership, int strength, string territoryName){
            this.value = value;
            this.econ = econ;
            this.ownership = ownership;
            this.strength = strength;
            this.name = territoryName;
        }

    }

    //list of all territories
    public List<Territory> territories = new List<Territory>();

    //territories belonging to the player
    public List<Territory> playerTerritories = new List<Territory>();

    void Start(){
        //fills the list of territories
        territories.AddRange(new Territory[] { oneistan, twoistan, threeistan, fouristan, fiveistan });

        // updates the score and logs it
        UpdateScore();


        CheckForBattle();

        UpdateStatus();
    }

    void UpdateScore(){
        //fills playerTerritories based on the territory's ownership value
        DevideTerritories();
        gameScore = 0;
        // sums the econ and value of all territories, adding it to their score and the overal player's gameScore
        foreach (Territory i in playerTerritories)
        {
            i.score = i.econ + i.value;
            gameScore += i.score;  // score -= warFatigue
        }
    }

    void DevideTerritories(){
        playerTerritories.Clear();
        foreach (Territory i in territories)
        {
            if (i.ownership == 0) { playerTerritories.Add(i); }

        }
    }

    void UpdateStatus(){
        statusText.text = "Status: Score = " + gameScore + "\n" + "Friendly Territories: ";
        foreach (Territory i in playerTerritories) {
            statusText.text += "\n" + i.name.ToUpper() + " Economy: " + i.econ + " Value: " + i.value + " Strength: " + i.strength;
            if (i.inBattleWith != null) {statusText.text += " Attacking: " + i.inBattleWith.name;}
            statusText.text += " Score " + i.score;
        }
        statusText.text += "\n" + "Enemy territories:";
        foreach (Territory i in territories) {
            if (i.ownership == 1) {
                statusText.text += "\n" + i.name.ToUpper() + " Economy: " + i.econ + " Value: " + i.value + " Strength: " + i.strength;
                if (i.inBattleWith != null) {statusText.text += " Attacking: " + i.inBattleWith.name;}
                statusText.text += " Score " + i.score;
            }
        }
    }

    //Processes the next turn
    public void Turn(){
        CheckForBattle();
        FightBattles();
        UpdateScore();
        Reinforce();
        UpdateStatus();
    }

    //starts a fight, m8
    //part of the random background war
    public void CheckForBattle(){
        //minimum ammount required to initiate a battle. Lowered each time. So that higher ratios are chosen first
        float min = 5f;

        //number of times ratios are compared to minimum
        int reps = 10;

        if (numberOfBattles < maximumBattles){

            //loops through and compares the ratios of the territory strengths
            while (reps > 0){
                foreach (Territory i in territories){
                    foreach (Territory j in territories){
                        if (i.strength * Random.Range(0.8f, 1.0f) / j.strength > min 
                        && i.ownership != j.ownership && !(i.inBattle) && !(i.underAttack)){
                            StartBattle(i, j);

                            //ends loop
                            return;
                        }
                    }
                }
                min *= .87f;
                reps -= 1;

            }
        }
    }

    void StartBattle(Territory t1, Territory t2){
        t1.inBattleWith = t2;
        t1.inBattle = true;
        t2.inBattle = true;
        t2.underAttack = true;
        numberOfBattles++;
        Debug.Log("Forces in " + t1.name + " have launched an attack on " + t2.name + ".");

    }

    //Simulates one turn of battle
    void FightBattles(){
        foreach (Territory atk in territories){
            if (atk.inBattleWith != null){
                Territory def = atk.inBattleWith;
                float atkFloat = atk.strength;
                float defFloat = def.strength;
                float strengthRat = atkFloat / defFloat;
                int defLosses = Mathf.RoundToInt(atk.strength / battleRetardation);

                int atkLosses = Mathf.RoundToInt(defLosses / Mathf.Pow(strengthRat, winnerAdvantage));
                
                if (def.strength > defLosses && atk.strength > atkLosses){
                    def.strength -= defLosses;
                    atk.strength -= atkLosses;
                    Debug.Log(atk.name + " lost " + atkLosses + " strength and " + def.name + " lost "
                    + defLosses + " strength. They are now at " + atk.strength + " and " + def.strength + " respectively");
                    Debug.Log(750 - 216);
                }
                //if the attacker wins               
                else if (atk.strength > 0) { EndBattle(atk, def); }
                else if (def.strength > 0) { EndBattle(def, atk); }
                else { Debug.Log("Something fucked up"); }
            }
        }
    }

    void EndBattle(Territory winner, Territory loser){
        loser.strength = 0;
        loser.ownership = winner.ownership;
        winner.inBattleWith = null;
        loser.inBattle = false;
        winner.inBattle = false;
        loser.underAttack = false;
        foreach (Territory i in territories){
            if (i.inBattleWith == loser){
                loser.inBattle = true;
                loser.underAttack = true;
            }
        }
        foreach (Territory i in territories){
            if (i.inBattleWith == winner){
                winner.inBattle = true;
            }
        }
        Debug.Log("Forces in " + loser.name + " surrendered to the invading army of " + winner.name);
        numberOfBattles--;
    }
    
    void Reinforce() {
        foreach (Territory i in playerTerritories) {
            if (i.underAttack == false) { i.strength += Mathf.RoundToInt(i.econ * econPower); }
        }
    }
}