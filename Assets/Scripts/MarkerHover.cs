using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarkerHover : MonoBehaviour
{

    [SerializeField] private GameObject moveTarget;
    [SerializeField] private GameObject fireTarget;
    [SerializeField] private GameObject targetParent;
    [SerializeField] private float zoomOffset = 1.50f;


    void OnMouseEnter()
    {
        transform.localScale = transform.localScale*zoomOffset;
    }

    void OnMouseExit()
    {
        transform.localScale = transform.localScale/zoomOffset;
    }

    void OnMouseDown()
    {
        Vector3 temp = new Vector3(transform.position.x, transform.position.y, 0f);
        TargetAssignHelper.tah.InstTarget(temp);
    }

}
