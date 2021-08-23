using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



// the vidal data of single character
[Serializable]
public class VitalData{
    public List<float> health = new List<float>();
    public int golds;
    public string name;
}


// the actionS of ALL characterS
[Serializable]
public class Episode{
    public List<Roleplay> roleplays = new List<Roleplay>();
}


// the actionS of one character
[Serializable]
public class Roleplay{
    public int cindex=-1;
    public List<Action> actions = new List<Action>();
}


// The single action data
[Serializable]
public class Action{
    public string type="0"; // (move | fire)
    public GameObject gunTypeObj; 
    public Vector3 target = new Vector3 (-999f,-999f,-999f);     public GameObject targetObj;
    public string ser; // Serialization of previous data
}


