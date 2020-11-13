using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetermineTarget : MonoBehaviour
{
    [SerializeField] private ActionsData aD;

    [SerializeField] private GameObject movTarget;
    [SerializeField] private GameObject fireTarget;
    GameObject tempTarget;

    Rigidbody2D temprb;

    Vector2 velocity;
 

    // Start is called before the first frame update
    void Start()
    {
        if(aD.actions[aD.actionTempNum].type=="fire"){
            tempTarget=fireTarget;
        }
        else if(aD.actions[aD.actionTempNum].type=="move"){
            tempTarget=movTarget;
        }
        temprb=tempTarget.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1)){
            velocity = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        }
    }

    void FixedUpdate() {
        temprb.MovePosition(velocity);
    }
}
