using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetHover : MonoBehaviour
{

    public GameObject moveTarget;
    public GameObject fireTarget;

    public GameObject targetParent;

    public float bigOffset = 1.50f;



    void OnMouseEnter()
    {
        transform.localScale = transform.localScale*bigOffset;
    }

    void OnMouseExit()
    {
        transform.localScale = transform.localScale/bigOffset;
    }

    void OnMouseDown()
    {
        Vector3 temp = new Vector3(transform.position.x, transform.position.y, 0f);
        TargetAssignHelper.tah.InstTarget(temp);
    }


}
