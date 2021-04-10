using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_drag : MonoBehaviour
{
    public string targetType;
    private Camera mainCamera;
    private float z;
    // Start is called before the first frame update
    void Start()
    {
       mainCamera = Camera.main;
       z = transform.position.z;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDrag(){
        Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = z;
        transform.position = worldPos;
    }

    // private void OnMouseUp()
    // {
        // GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[GM.gm.actionIndex].target = gameObject;
    // }

}
