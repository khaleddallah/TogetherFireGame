using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uib_general : MonoBehaviour
{

    [SerializeField] private GameObject actionSelectingUnit;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyPressed(){
        actionSelectingUnit.SetActive(false);
    }
}
