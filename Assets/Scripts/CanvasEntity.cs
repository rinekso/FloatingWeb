using System.Collections;
using System.Collections.Generic;
using TLab.Android.WebView;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasEntity : MonoBehaviour
{
    [SerializeField]
    GameObject border;
    [SerializeField]
    TLabWebView webView;
    [SerializeField]
    RectTransform canvasParent;
    [SerializeField]
    BoxCollider collider;
    float scale,startWidthCanvas,startHeighCanvas;
    void Start(){
        scale = transform.GetChild(0).localScale.x;
    }
    public void ShowBorder(bool val){
        border.SetActive(val);
    }
    public void InitResize(){
        startWidthCanvas = canvasParent.sizeDelta.x;
        startHeighCanvas = canvasParent.sizeDelta.y;
        print(startWidthCanvas+"/"+startHeighCanvas);
    }
    public void ResizeWeb(float width, float height){
        float minimum = 750;
        if(startWidthCanvas+width < minimum || startHeighCanvas+height < minimum)
            return;

        canvasParent.sizeDelta = new Vector2(startWidthCanvas+width,startHeighCanvas+height);
        webView.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.RoundToInt(startWidthCanvas+width),Mathf.RoundToInt(startHeighCanvas+height-100f));
        webView.WebWidth = Mathf.RoundToInt(startWidthCanvas + width);
        webView.WebHeight = Mathf.RoundToInt(startHeighCanvas + height - 100f);
        
        collider.size = new Vector2(scale*(startWidthCanvas+width),scale*(startHeighCanvas+height));
        float y = (startHeighCanvas+height)/2*scale+.1f;
        canvasParent.localPosition = new Vector3(0,y,0);
        collider.transform.localPosition = new Vector3(0,y,0);
    }
    public void Apply(){
        webView.RestartWebView();
    }
}
