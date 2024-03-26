using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Meta.XR.BuildingBlocks;
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
    private Object _leftHand;
    [SerializeField]
    GameObject browserTab, visual;
    [SerializeField]
    Camera cam;
    // [SerializeField]
    // GameObject canvas, coll;
    public IHand LeftHand { get; set; }
    bool firstVisible = false;
    public bool Visible{
        get => firstVisible;
    }
    bool _virtualKeyboard = false;
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
    Image _keyboardBackground;
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
    }

    // Update is called once per frame
    void Update()
    {
        Pose wristPose;
        if (LeftHand.GetJointPose(HandJointId.HandWristRoot, out wristPose))
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

    }
    public void ToggleVirtualKeyboard(){
        VirtualKeyboard = !_virtualKeyboard;
        _keyboardBackground.color = VirtualKeyboard ? new Color(128,128,128,255) : new Color(95,186,226,255);
    }
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
