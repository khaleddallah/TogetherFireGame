using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.FindGameObjectWithTag("canvas").GetComponent<Canvas>();
    }


    public void OnPointerDown(PointerEventData eventData){
        Debug.Log("OnPointerDown");
    }

    public void OnDrag(PointerEventData eventData){
        Debug.Log("OnDrag");
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }
 
    public void OnBeginDrag(PointerEventData eventData){
        Debug.Log("OnBeginDrag");
    }


    public void OnEndDrag(PointerEventData eventData){
        Debug.Log("OnEndDrag");        
        Debug.Log("## pos :"+rectTransform.TransformPoint(rectTransform.transform.position));
        Debug.Log("## Cam pos :"+Camera.main.WorldToViewportPoint(rectTransform.TransformPoint(rectTransform.transform.position)));
    }
         



}
