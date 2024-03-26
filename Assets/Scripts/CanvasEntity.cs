using System.Collections;
using System.Collections.Generic;
using TLab.Android.WebView;
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
    Transform coll, helperLookat;
    [SerializeField]
    CanvasAnchor canvasAnchor;
    [SerializeField]
    bool isUnlock = true;
    public bool IsUnlock{
        get => isUnlock;
    }
    [SerializeField]
    GameObject[] otherMenu;
    [SerializeField]
    float minimum = 750;
    float scale,startWidthCanvas,startHeighCanvas;
    void Start(){
        scale = transform.GetChild(0).localScale.x;
    }
    public void ShowBorder(bool val){
        if(!isUnlock)
            border.SetActive(false);
        else
            border.SetActive(val);
    }
    public void InitResize(){
        startWidthCanvas = canvasParent.sizeDelta.x;
        startHeighCanvas = canvasParent.sizeDelta.y;
        print(startWidthCanvas+"/"+startHeighCanvas);
    }
    public void ResizeWeb(float width, float height){
        float widthResult, heighResult;
        widthResult = startWidthCanvas+width;
        heighResult = startHeighCanvas+height;
        if(startWidthCanvas+width < minimum)
            widthResult = minimum;
        if(startHeighCanvas+height < minimum)
            heighResult = minimum;

        canvasParent.sizeDelta = new Vector2(widthResult,heighResult);
        webView.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.RoundToInt(widthResult),Mathf.RoundToInt(heighResult-200f));
        webView.WebWidth = Mathf.RoundToInt(startWidthCanvas + width);
        webView.WebHeight = Mathf.RoundToInt(startHeighCanvas + height - 200f);
        
        coll.localScale = new Vector3(scale*(widthResult),scale*(heighResult),0.01f);
        float y = (heighResult)/2*scale+.1f;
        helperLookat.localPosition = new Vector3(0,y*transform.localScale.y,0);
        canvasParent.localPosition = new Vector3(0,y,0);
        coll.transform.localPosition = new Vector3(0,y,0);
    }
    public void LookAt(Vector3 pos){
        helperLookat.transform.LookAt(pos);
        transform.rotation = helperLookat.rotation;
    }
    public void Apply(){
        print("resize Apply "+webView.WebWidth+"/"+webView.WebHeight);
        webView.RestartWebView();
    }
    public void CloseButton(){
        Destroy(transform.parent.gameObject);
    }
    public void ToggleLock(){
        isUnlock = !isUnlock;
        Lock(isUnlock);
    }
    public void Lock(bool val){
        canvasAnchor.gameObject.SetActive(val);
        if(!val) border.SetActive(false);
        for (int i = 0; i < otherMenu.Length; i++)
        {
            otherMenu[i].SetActive(val);
        }
    }
}
