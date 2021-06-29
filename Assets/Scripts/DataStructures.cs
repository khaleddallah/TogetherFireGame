using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;





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
    public Vector3 target = new Vector3 (-999f,-999f,-999f); // target of the action either move or fire
    public GameObject targetObj;
    public string ser; // Serialization of previous data
}


