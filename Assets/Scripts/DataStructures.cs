using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameData{
    public int playerIndex;
    public List<Episode> episodes = new List<Episode>();
}

[Serializable]
public class Episode{
    public List<Roleplay> roleplays = new List<Roleplay>();
}

[Serializable]
public class Roleplay{
    // here
    public List<Action> actions = new List<Action>();
}

[Serializable]
public class Action{
    public string type="0"; // move | fire
    // public string gunType="0"; // specific property
    public GameObject gunTypeObj; 
    public GameObject target;

}


