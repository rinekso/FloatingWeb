using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HelperResizeBrowser : MonoBehaviour
{
    RectTransform _target;
    public void InitResize(RectTransform target){
        _target = target;
    }
    public void FinishResize(){
        _target = null;
    }
    void Update(){
        if(_target){
            transform.position = _target.position;
        }
    }
}
