using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using TLab.Android.WebView;
using TLab.InputField;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public static MenuScript Instance;
    [SerializeField, Interface(typeof(IHand))]
    private Object _leftHand, _leftHandController;
    [SerializeField]
    GameObject browserTab, visual;
    [SerializeField]
    Camera cam;
    [System.Serializable]
    public struct BrowserData{
        public Vector3 ContainerPosition;
        public Vector3 PointAnchorPosition;
        public Quaternion CanvasRotation;
        public Vector2 CanvasParent;
        public string url;
    }
    [System.Serializable]
    public struct BrowserDataContainer{
        public List<BrowserData> BrowserData;
    }
    BrowserDataContainer browserData;
    // [SerializeField]
    // GameObject canvas, coll;
    public IHand LeftHand { get; set; }
    public IHand LeftHandController { get; set; }
    [SerializeField]
    OVRHand OvrHand;
    bool firstVisible = false;
    public bool Visible{
        get => firstVisible;
    }
    bool _virtualKeyboard = false, _toggleLock = true;
    public bool ToggleLock{
        set{
            _toggleLock = value;
            CanvasEntity[] canvas = FindObjectsOfType<CanvasEntity>();
            for (int i = 0; i < canvas.Length; i++)
            {
                canvas[i].Lock(value);
            }
        }
        get{
            return _virtualKeyboard;
        }
    }
    public bool VirtualKeyboard{
        set{
            _virtualKeyboard = value;
            TLabVKeyborad[] keyboards = FindObjectsOfType<TLabVKeyborad>();
            for (int i = 0; i < keyboards.Length; i++)
            {
                keyboards[i].Disable = value;
            }
        }
        get{
            return _virtualKeyboard;
        }
    }
    [SerializeField]
    GameObject _keyboardLock, _keyboardLockOff;
    void Awake(){
        Instance = this;
    }
    public void SpawnBrowser(){
        float distance = .5f;
        GameObject go = Instantiate(browserTab,transform.position+Camera.main.transform.forward*distance,Quaternion.LookRotation(Camera.main.transform.position));
        go.GetComponentInChildren<CanvasEntity>().LookAt(Camera.main.transform.position);
        // HideMenu();
    }
    // public void HideMenu(){
    //     transform.localScale = Vector3.zero;
    // }
    // public void ShowMenu(Vector3 pos){
    //     transform.localScale = Vector3.one;
    //     transform.LookAt(Camera.main.transform);
    //     transform.position = pos;
    //     // SwitchPos(isRight);
    // }
    // void SwitchPos(bool val){
    //     float x = Mathf.Abs(canvas.transform.localPosition.x);
    //     canvas.transform.localPosition = new Vector3(val?x:-x,canvas.transform.localPosition.y,0);
    //     coll.transform.localPosition = new Vector3(val?x:-x,coll.transform.localPosition.y,0);
    // }
    // Start is called before the first frame update
    void Start()
    {
        LeftHand = _leftHand as IHand;
        LeftHandController = _leftHandController as IHand;
        if(PlayerPrefs.HasKey("BrowserData")){
            LoadLayout();
        }
    }

    // Update is called once per frame
    void Update()
    {
        IHand hand;
        // print("Hand "+OvrHand.IsTracked);
        if(OvrHand.IsTracked)
            hand = LeftHand;
        else
            hand = LeftHandController;

        Pose wristPose;
        if (hand.GetJointPose(HandJointId.HandWristRoot, out wristPose))
        {
            transform.SetPose(wristPose);
            bool visible = IsTargetVisible(cam,Camera.main.gameObject);
            if(firstVisible != visible){
                firstVisible = visible;
                visual.SetActive(visible);
                visual.transform.localScale = visible?Vector3.one:Vector3.zero;
            }
            
        }
    }
    bool IsTargetVisible(Camera c,GameObject go)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = go.transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }
        return true;
    }
    public void SaveLayout(){
        browserData = new BrowserDataContainer();
        browserData.BrowserData = new List<BrowserData>();
        GameObject[] browsers = GameObject.FindGameObjectsWithTag("Browser");
        for(int i = 0; i<browsers.Length; i++){
            BrowserData dataTemp;
            CanvasEntity canvasEntity = browsers[i].GetComponentInChildren<CanvasEntity>();
            dataTemp.ContainerPosition = browsers[i].transform.position;
            dataTemp.PointAnchorPosition = browsers[i].transform.GetChild(0).position;
            dataTemp.CanvasRotation = canvasEntity.transform.rotation;
            dataTemp.CanvasParent = canvasEntity.CanvasParent.sizeDelta;
            dataTemp.url = canvasEntity.GetURL();
            browserData.BrowserData.Add(dataTemp);
        }
        PlayerPrefs.SetString("BrowserData",JsonUtility.ToJson(browserData));
    }
    public void LoadLayout(){
        GameObject[] browsers = GameObject.FindGameObjectsWithTag("Browser");
        for (int i = 0; i < browsers.Length; i++)
        {
            Destroy(browsers[i]);
        }
        StartCoroutine(InstantiateWeb());
    }
    IEnumerator InstantiateWeb(){
        BrowserDataContainer BrowserDataSaved = JsonUtility.FromJson<BrowserDataContainer>(PlayerPrefs.GetString("BrowserData"));
        foreach (var item in BrowserDataSaved.BrowserData)
        {
            GameObject go = Instantiate(browserTab);
            CanvasEntity canvasEntity = go.GetComponentInChildren<CanvasEntity>();
            go.transform.position = item.ContainerPosition;
            go.transform.GetChild(0).position = item.PointAnchorPosition;
            canvasEntity.transform.rotation = item.CanvasRotation;
            bool wait = true;
            while (wait)
            {
                wait = !canvasEntity.IsWebInit();
                yield return new WaitForEndOfFrame();
            }
            canvasEntity.ResizeAndLoad(item.CanvasParent.x,item.CanvasParent.y,item.url);
        }
    }
    public void LockToggle(){
        ToggleLock = !_toggleLock;
        _keyboardLock.SetActive(!_toggleLock);
        _keyboardLockOff.SetActive(_toggleLock);
    }
    // public void ToggleVirtualKeyboard(){
    //     VirtualKeyboard = !_virtualKeyboard;
    //     _keyboardBackground.color = _virtualKeyboard ? Color.white : new Color(95,186,226,255);
    // }
    public void ResetSave(){
        PlayerPrefs.DeleteAll();
    }
    public void ClearCache(){
        TLabWebView[] webview = FindObjectsOfType<TLabWebView>();
        for (int i = 0; i < webview.Length; i++)
        {
            webview[i].ClearCache(true);
        }
    }
    public void ClearCookie(){
        TLabWebView[] webview = FindObjectsOfType<TLabWebView>();
        for (int i = 0; i < webview.Length; i++)
        {
            webview[i].ClearCookie();
        }
    }
}
