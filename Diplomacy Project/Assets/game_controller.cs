using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class game_controller : MonoBehaviour {

    //the player's score
    public int gameScore;
    int numberOfBattles = 0;
    public int maximumBattles;
    public float winnerAdvantage;
    public float battleRetardation;

    //creates a bunch of territories
    public Territory oneistan = new Territory(500, 1000, 0, 1000, "Oneistan");
    Territory twoistan = new Territory(200, 450, 0, 1000, "Twoistan");
    Territory threeistan = new Territory(1000, 1500, 0, 1000, "Threeistan");
    Territory fouristan = new Territory(500, 1500, 1, 1500, "Fouristan");
    Territory fiveistan = new Territory(500, 2000, 1, 1000, "Fiveistan");

    //class of the territories sides fight over
    public class Territory {

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
        public Territory(int value, int econ, int ownership, int strength, string territoryName) {
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
    List<Territory> playerTerritories = new List<Territory>();
   
    void Start() {
        //fills the list of territories
        territories.AddRange(new Territory[] { oneistan, twoistan, threeistan, fouristan, fiveistan });

        //fills playerTerritories based on the territory's ownership value
        DevideTerritories();

        // updates the score and logs it
        UpdateScore();

        CheckForBattle();
    }

    void UpdateScore()
    {
        gameScore = 0;
        // sums the econ and value of all territories, adding it to their score and the overal player's gameScore
        foreach (Territory i in playerTerritories){
            i.score = i.econ + i.value;
            gameScore += i.score;  // score -= warFatigue
        }

        Debug.Log(gameScore);
    }

    void DevideTerritories() {
        foreach (Territory i in territories) {
            if (i.ownership == 0) { playerTerritories.Add(i); }
        }
    }



    //Processes the next turn
    public void Turn(){
        CheckForBattle();
        UpdateScore();
        FightBattles();
    }

    //starts a fight, m8
    //part of the random background war
    public void CheckForBattle() {
        //minimum ammount required to initiate a battle. Lowered each time. So that higher ratios are chosen first
        float min = 5f;

        //number of times ratios are compared to minimum
        int reps = 10;

        //for the loop
        bool finished = false;

        if (numberOfBattles < maximumBattles) {

            //loops through and compares the ratios of the territory strengths
            while (finished == false && reps > 0) {
                foreach (Territory i in territories) {
                    foreach (Territory j in territories) {
                        if (i.strength * Random.Range(0.8f, 1.0f) / j.strength > min && i.ownership != j.ownership && !(i.inBattle) && !(i.underAttack)) {
                            StartBattle(i, j);

                            //ends loop
                            finished = true;
                        }
                    }
                }
                min *= .87f;
                reps -= 1;

            }
        }
    }

    void StartBattle(Territory t1, Territory t2) {
        t1.inBattleWith = t2;
        t1.inBattle = true;
        numberOfBattles++;
        Debug.Log("Forces in " + t1.name + " have launched an attack on " + t2.name + ".");

    }

    //Simulates one turn of battle
    void FightBattles() {
        foreach (Territory atk in territories) {
            if (atk.inBattle) {
                Territory def = atk.inBattleWith;
                Debug.Log(atk.strength + "    " + def.strength);
                float atkFloat = atk.strength;
                float defFloat = def.strength;
                float strengthRat = atkFloat / defFloat;
                Debug.Log(strengthRat);
                int defLosses = Mathf.RoundToInt(atk.strength /battleRetardation);
                def.strength -= defLosses;
                int atkLosses = Mathf.RoundToInt(defLosses / Mathf.Pow(strengthRat, winnerAdvantage));
                atk.strength -= atkLosses;
                Debug.Log(atk.name + " lost " + atkLosses + " strength and " + def.name + " lost " + defLosses + " strength. They are now at " + atk.strength+ " and " + def.strength + " respectively");

            }
        }
    }
}