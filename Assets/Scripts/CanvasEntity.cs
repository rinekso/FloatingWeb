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
    public RectTransform CanvasParent{
        get{ return canvasParent; }
    }
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
    int webWidth, webHeight;
    [SerializeField]
    GameObject lockOn,lockOff;
    void Awake(){
        scale = transform.GetChild(0).localScale.x;
    }
    void Start(){
    }
    public string GetURL(){
        return webView.GetUrl();
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
    public bool IsWebInit(){
        return webView.IsInitialized();
    }
    public void ResizeAndLoad(float width, float height, string url){
        ResizeExact(width,height);
        Apply();
        webView.LoadUrl(url);
    }
    void ResizeExact(float width, float height){
        canvasParent.sizeDelta = new Vector2(width,height);
        webView.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.RoundToInt(width),Mathf.RoundToInt(height-200f));
        webWidth = Mathf.RoundToInt(startWidthCanvas + width);
        webHeight = Mathf.RoundToInt(startHeighCanvas + height - 200f);
        
        coll.localScale = new Vector3(scale*width,scale*height,0.01f);
        float y = height/2*scale+.1f;
        helperLookat.localPosition = new Vector3(0,y*transform.localScale.y,0);
        canvasParent.localPosition = new Vector3(0,y,0);
        coll.transform.localPosition = new Vector3(0,y,0);
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
        webWidth = Mathf.RoundToInt(startWidthCanvas + width);
        webHeight = Mathf.RoundToInt(startHeighCanvas + height - 200f);
        
        coll.localScale = new Vector3(scale*widthResult,scale*heighResult,0.01f);
        float y = heighResult/2*scale+.1f;
        helperLookat.localPosition = new Vector3(0,y*transform.localScale.y,0);
        canvasParent.localPosition = new Vector3(0,y,0);
        coll.transform.localPosition = new Vector3(0,y,0);
    }
    public void LookAt(Vector3 pos){
        helperLookat.transform.LookAt(pos);
        transform.rotation = helperLookat.rotation;
    }
    public void Apply(){
        print("resize Apply "+webWidth+"/"+webHeight);
        webView.ResizeWeb(webWidth, webHeight);
    }
    public void CloseButton(){
        Destroy(transform.parent.gameObject);
    }
    public void ToggleLock(){
        isUnlock = !isUnlock;
        Lock(isUnlock);
    }
    public void Lock(bool val){
        isUnlock = val;
        canvasAnchor.gameObject.SetActive(val);
        if(!val) border.SetActive(false);
        for (int i = 0; i < otherMenu.Length; i++)
        {
            otherMenu[i].SetActive(val);
        }

        lockOn.SetActive(val);
        lockOff.SetActive(!val);
    }
}
