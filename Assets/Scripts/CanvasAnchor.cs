using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAnchor : MonoBehaviour
{
    [SerializeField]
    CanvasEntity canvas;
    bool faceCanvas = false;
    void Update(){
        if(faceCanvas){
            Vector3 pos = Camera.main.transform.position;
            // pos.y = pos.y*5/8;
            canvas.LookAt(pos);
        }
    }
    public void SetFace(bool val){
        faceCanvas = val;
    }
}
