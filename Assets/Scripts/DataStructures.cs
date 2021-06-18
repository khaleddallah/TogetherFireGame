using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



// the data of all players at a complete game.
[Serializable]
public class GameData{
    public int playerIndex; // the index of the player
    public List<Episode> episodes = new List<Episode>(); // The episodes data
    public List<VitalData> vitalDatas = new List<VitalData>(); // the vital data of all players 
}


// the vidal data of single character
[Serializable]
public class VitalData{
    public float health;
    public int golds;
}


// the actionS of ALL characterS
[Serializable]
public class Episode{
    public List<Roleplay> roleplays = new List<Roleplay>();
}


// the actionS of one character
[Serializable]
public class Roleplay{
    public List<Action> actions = new List<Action>();
}


// The single action data
[Serializable]
public class Action{
    public string type="0"; // type of the action (move | fire)
    public GameObject gunTypeObj; // type of the gun 
    public GameObject target; // target of the action either move or fire
    public string ser; // Serialization of previous data
}


