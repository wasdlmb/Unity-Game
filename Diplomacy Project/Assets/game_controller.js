/*#pragma strict

var gameScore : int;
var value : int;
var econ : int;
var ownership : int;
var score : int;
function territory(value, econ, ownership) {
	this.value = value;
	this.econ = econ;
	this.ownership = ownership;
	this.score = 0;
}
var oneistan : territory = new territory(500, 1000, 0);
var twoistan = new territory(200, 450, 0);
var threeistan = new territory(1000, 1500, 0);
var fouristan = new territory(500, 1500, 1);
var fiveistan = new territory(500, 2000, 1);

var playerTerritories = [oneistan, twoistan];

function updateScore() {
            gameScore = 0;
            for (territory in playerTerritories) {
                territory.score = territory.econ + territory.value;
               gameScore += territory.score;  //          score -= warFatigue
		    }
}

function Start () {
	updateScore;
	debug.Log(gameScore);
}

function Update () {

}

/*    void Start() {
        public static void updateScore() {
//            gameScore = 0;
//            foreach (territory i in playerTerritories) {
//                i.score = i.econ + i.value;
//                gameScore += i.score;  //          score -= warFatigue
//            }
        }
    }

    void Update () {
	    
	}
}*/
