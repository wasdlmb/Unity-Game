using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class game_controller : MonoBehaviour {

    //the player's score
    public int gameScore;

    public int numberOfBattles;

    public int maximumBattles;

    //class of the territories sides fight over
    public class Territory {

        //value: how much your side wants it(ethnic history, religion, etc.)
        //econ: how much the teritory can produce
        //ownership: 0 is player, 1 is enemy, maybe 2 for independent
        //score: the sum total of these, used for calculating game score
        public int value, econ, ownership, score;
        public float strength;
        public string name;

        //constructor
        public Territory(int value, int econ, int ownership, float strength, string territoryName) {
            this.value = value;
            this.econ = econ;
            this.ownership = ownership;
            this.score = 0;
            this.strength = strength;
            this.name = territoryName;


        }

    }
    //creates a bunch of territories
    Territory oneistan = new Territory(500, 1000, 0, 1000, "Oneistan");
    Territory twoistan = new Territory(200, 450, 0, 1000, "Twoistan");
    Territory threeistan = new Territory(1000, 1500, 0, 750, "Threeistan");
    Territory fouristan = new Territory(500, 1500, 1, 1500, "Fouristan");
    Territory fiveistan = new Territory(500, 2000, 1, 1000, "Fiveistan");

    //list of all territories
    List<Territory> territories = new List<Territory>();

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
            gameScore += i.score;  //          score -= warFatigue
        }
        Debug.Log(gameScore);

    }

    void DevideTerritories() {
        foreach (Territory i in territories) {
            if (i.ownership == 0) { playerTerritories.Add(i); }
        }
    }

    //not used
    void Update(){

    }

    //starts a fight, m8
    //part of the random background war
    void CheckForBattle() {
        float min = 5f;
        int reps = 10;
        bool finished = false;
        if (numberOfBattles < maximumBattles) {
            while (finished == false && reps > 0) {
                foreach (Territory i in territories) {
                    foreach (Territory j in territories) {
                        if (i.strength * Random.Range(0.8f, 1.1f) / j.strength > min && i.ownership != j.ownership) {
                            StartBattle(i, j);
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
        Debug.Log("A battle has been started between forces in " + t1.name + " and " + t2.name + ".");
    }
}/*
        While(bool finished = 0 && reps > 0)

            For each territory i in territories
                For each territory j in territories
                    Ratio = i.strength / j.strength;
    If ratio*mathf.RandomRange(.8, 1) > min
StartBattle(i, j);
    Finished = true;
Min *=.86
Reps -= 1
*/