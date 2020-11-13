using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GunsButton : MonoBehaviour
{
    [SerializeField] private RectTransform movFireList;
    [SerializeField] private RectTransform gunList;
    [SerializeField] private float offset;
    [SerializeField] private ActionsData aD;

    public GameObject tfire;

    public void ChooseGun(){
        //hide lists
        movFireList.gameObject.SetActive(false);
        gunList.gameObject.SetActive(false);

        //save type of gun
        aD.actions[aD.actionTempNum-1].gun=transform.name;

        //change label

        //run choose ShootTarget process
        GameObject x = Instantiate(target) as GameObject;
        x.transform.position = new Vector3(aD.actionTempNum-2.0f, 0f, 0f);

    }
}
