using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasAnchor : MonoBehaviour
{
    [SerializeField]
    AlwaysFace canvas;
    public void SetFace(){
        Vector3 pos = Camera.main.transform.position;
        pos.y = pos.y*1/2;
        canvas.SetFaceByPosition(pos);
    }
}
