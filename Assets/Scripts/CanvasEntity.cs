using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
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
    Canvas canvas;
    [SerializeField]
    GameObject startSphere;
    GameObject player;
    void Awake(){
        scale = transform.GetChild(0).localScale.x;
    }
    void Start(){
        canvas = GetComponent<Canvas>();
        pointableCanvas = GetComponent<PointableCanvas>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public string GetURL(){
        return webView.GetUrl();
    }
    public void ShowBorder(bool val){
        if(!isUnlock)
            border.GetComponent<CanvasGroup>().alpha = .2f;
        else
            border.GetComponent<CanvasGroup>().alpha = val ? 1 : .2f;
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
        print("exact "+width+"/"+height+"");
        canvasParent.sizeDelta = new Vector2(width+200,height);
        webView.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.RoundToInt(width),Mathf.RoundToInt(height-200f));
        webWidth = Mathf.RoundToInt(startWidthCanvas + width);
        webHeight = Mathf.RoundToInt(startHeighCanvas + height - 200f);
        
        coll.localScale = new Vector3(scale*(width+200),scale*(height),0.01f);
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

        canvasParent.sizeDelta = new Vector2(widthResult+200,heighResult);
        webView.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.RoundToInt(widthResult),Mathf.RoundToInt(heighResult-200f));
        webWidth = Mathf.RoundToInt(widthResult);
        webHeight = Mathf.RoundToInt(heighResult - 200f);
        
        coll.localScale = new Vector3(scale*(widthResult+200),scale*(heighResult),0.01f);
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
        if(!val) border.GetComponent<CanvasGroup>().alpha = .2f;
        for (int i = 0; i < otherMenu.Length; i++)
        {
            otherMenu[i].SetActive(val);
        }

        lockOn.SetActive(val);
        lockOff.SetActive(!val);
    }
    public bool Resize{
        set{ drag = value; }
        get { return drag; }
    }
    public void InitResizeViaPointer(bool value = false){
        InitResize();
        reverse = value;
        PointableCanvas canvas = GetComponent<PointableCanvas>();
        helperResize.position = canvas.SelectingPoints[0].position;
        startSphere.transform.position = canvas.SelectingPoints[0].position;
        // startPos = transform.InverseTransformPoint(canvas.SelectingPoints[0].position);
        drag = true;
        apply = false;
    }
    // Vector3 startPos;
    [SerializeField]
    Transform helperResize;
    bool drag=false, reverse = false, apply;
    PointableCanvas pointableCanvas;
    [SerializeField]
    float scaleTemp = 1500;
    void Update(){
        if(drag && pointableCanvas.SelectingPointsCount > 0){
            if(pointableCanvas.SelectingPointsCount == 1 && pointableCanvas.SelectingPoints[0].position != player.transform.position){
                helperResize.position = pointableCanvas.SelectingPoints[0].position;
                float horizontalDiff = startSphere.transform.localPosition.x-helperResize.localPosition.x;
                float verticalDiff = startSphere.transform.localPosition.y-helperResize.localPosition.y;
                print("resize scale "+horizontalDiff*scaleTemp+"/"+verticalDiff*(scaleTemp*.5f));
                if(reverse)
                    ResizeWeb(horizontalDiff*scaleTemp,verticalDiff*(scaleTemp*.5f));
                else
                    ResizeWeb(-horizontalDiff*scaleTemp,-verticalDiff*(scaleTemp*.5f));
            }
        }else if(pointableCanvas.SelectingPointsCount == 0 && !apply){
            drag = false;
            apply = true;
            Apply();
        }
    }
}
