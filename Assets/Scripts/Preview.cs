using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview : MonoBehaviour
{
    public GameObject myChar;
    public ActionsData aD;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Submit(){
        Vector3 t = aD.actions[aD.actionTempNum-1].target.GetComponent<RectTransform>().transform.position;
        // Vector3 t2 =  Camera.main.WorldToViewportPoint(t);    
        myChar.transform.position = Camera.main.ViewportToWorldPoint(t);
    }
}
