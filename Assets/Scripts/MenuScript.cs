using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Meta.XR.BuildingBlocks;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public static MenuScript Instance;
    [SerializeField, Interface(typeof(IHand))]
    private Object _leftHand;
    [SerializeField]
    GameObject browserTab, visual;
    [SerializeField]
    Camera cam;
    [SerializeField]
    RayInteractor[] rayInteractors;
    // [SerializeField]
    // GameObject canvas, coll;
    public IHand LeftHand { get; set; }
    bool firstVisible = false;
    public bool Visible{
        get => firstVisible;
    }
    void Awake(){
        Instance = this;
    }
    public void SpawnBrowser(){
        Instantiate(browserTab,transform.position,Quaternion.LookRotation(Camera.main.transform.position));
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
            transform.position = wristPose.position;
            bool visible = IsTargetVisible(cam,Camera.main.gameObject);
            if(firstVisible != visible){
                firstVisible = visible;
                visual.SetActive(visible);
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
}
