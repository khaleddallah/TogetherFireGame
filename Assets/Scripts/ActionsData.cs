using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action{
    public string type="0"; // move | fire
    public string gun="0"; // specific property
    public GameObject target;
    public GameObject txt;
}

public class ActionsData : MonoBehaviour
{
    public List<Action> actions;
    public int actionTempNum;



    // Start is called before the first frame update
    void Start()
    {
        actions = new List<Action>(3);
        for (int i=0; i<=2; i++){
            Action x = new Action();
            actions.Add(x);
        }

        for (int i=0; i<=2; i++){
            Debug.Log(actions[i].type);
        }
        // Debug.Log("size actions is :"+actions.Capacity);
    }

}
