using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognized;
}
public class GestureDetection : MonoBehaviour
{
    public float threshold = 0.1f;
    public bool debugMode = true;
    public bool saveRightSkeleton = true;
    public OVRSkeleton skeletonRight;
    public OVRSkeleton skeletonLeft;
    public List<Gesture> gestures;
    private List<OVRBone> fingerBone;
    private Gesture previousGesture;
    // Resize Func
    CanvasEntity canvasTarget;
    float startDistanceHorizontal,startDistanceVertical;
    float startWidthCanvas,startHeighCanvas;
    [SerializeField]
    Transform helper;
    bool isFirstResize = false;
    bool isResizing = false;
    // Start is called before the first frame update
    void Start()
    {
        previousGesture = new Gesture();
    }
    // Update is called once per frame
    void Update()
    {
        if(debugMode && Input.GetKeyDown(KeyCode.Space)){
            fingerBone = new List<OVRBone>(saveRightSkeleton ? skeletonRight.Bones : skeletonLeft.Bones);
            Save(saveRightSkeleton ? skeletonRight : skeletonLeft);
        }else{
            Gesture currentGesture = Recognize(skeletonRight);
            bool hasRecognized = !currentGesture.Equals(new Gesture());

            if(hasRecognized && !currentGesture.Equals(previousGesture)){
                // print("new Gesture "+currentGesture.name);
                previousGesture = currentGesture;

                if(currentGesture.onRecognized != null)
                    currentGesture.onRecognized.Invoke();

                fingerBone = new List<OVRBone>(skeletonRight.Bones);
            }

            DetectCanvasInFront();
            if(canvasTarget != null){
                if(currentGesture.name == "PistolRight"){
                    Gesture leftGesture = Recognize(skeletonLeft);
                    // print("left "+leftGesture.name);
                    if(leftGesture.name == "PistolLeft"){
                        if(!isFirstResize){
                            print("Resize");
                            isFirstResize = true;
                            InitResize();
                        }
                        Resize();
                    }else{
                        if(isFirstResize) ApplyResize();
                        isFirstResize = false;                        
                    }
                }else{
                    if(isFirstResize) ApplyResize();
                    isFirstResize = false;
                }
            }
        }
    }
    // public void OpenMenuRight(){
    //     MenuScript.Instance.ShowMenu(GetFinger(skeletonRight, OVRSkeleton.BoneId.Hand_Index2).position, true);
    // }
    // public void OpenMenuLeft(){
    //     MenuScript.Instance.ShowMenu(GetFinger(skeletonLeft, OVRSkeleton.BoneId.Hand_Index2).position, false);
    // }
    void InitResize(){
        UpdateHelperChildPos();
        startDistanceHorizontal = DistanceH();
        startDistanceVertical = DistanceV();
        canvasTarget.InitResize();
        isResizing = true;
    }
    void Resize(){
        UpdateHelperChildPos();

        float horizontalCalculate = DistanceH(), verticalCalculate = DistanceV();

        float horizonDiff = horizontalCalculate-startDistanceHorizontal;
        float verticalDiff = verticalCalculate-startDistanceVertical;

        // print(startDistanceHorizontal+"/"+startDistanceVertical+" | "+horizontalCalculate+"/"+verticalCalculate);
        float scale = 2000;
        canvasTarget.ResizeWeb(horizonDiff*scale,verticalDiff*scale);
    }
    void UpdateHelperChildPos(){
        Transform tipLeft = GetFinger(skeletonLeft,OVRSkeleton.BoneId.Hand_Index1);
        Transform tipRight = GetFinger(skeletonRight,OVRSkeleton.BoneId.Hand_Index1);
        helper.transform.LookAt(Camera.main.transform);
        helper.transform.position = Vector3.Lerp(tipLeft.position,tipRight.position,0.5f);
        helper.GetChild(0).localPosition = helper.InverseTransformPoint(tipLeft.transform.position);
        helper.GetChild(1).localPosition = helper.InverseTransformPoint(tipRight.transform.position);
    }
    float DistanceH(){
        float horizontalCalculate = 0;
        Vector3 leftHorizontal = Vector3.zero, rightHorizontal = Vector3.zero;
        leftHorizontal.y = helper.GetChild(0).localPosition.y;
        rightHorizontal.y = helper.GetChild(1).localPosition.y;
        horizontalCalculate += Vector3.Distance(helper.GetChild(0).localPosition,leftHorizontal);
        horizontalCalculate += Vector3.Distance(helper.GetChild(1).localPosition,rightHorizontal);
        return horizontalCalculate;
    }
    float DistanceV(){
        float verticalCalculate = 0;
        Vector3 leftHorizontal = Vector3.zero, rightHorizontal = Vector3.zero;
        leftHorizontal.x = helper.GetChild(0).localPosition.x;
        rightHorizontal.x = helper.GetChild(1).localPosition.x;
        verticalCalculate += Vector3.Distance(helper.GetChild(0).localPosition,leftHorizontal);
        verticalCalculate += Vector3.Distance(helper.GetChild(1).localPosition,rightHorizontal);
        return verticalCalculate;
    }
    void ApplyResize(){
        isResizing = false;
        if(waitSatbleResize != null){
            StopCoroutine(waitSatbleResize);
        }
        waitSatbleResize = StartCoroutine(WaitStableResize(canvasTarget));
    }
    Coroutine waitSatbleResize;
    IEnumerator WaitStableResize(CanvasEntity target){
        yield return new WaitForSeconds(1);
        target.Apply();
        waitSatbleResize = null;
    }
    Transform GetFinger(OVRSkeleton skeletonTarget, OVRSkeleton.BoneId boneId){
        for (int i = 0; i < skeletonTarget.Bones.Count; i++)
        {
            if(skeletonTarget.Bones[i].Id == boneId)
                return skeletonTarget.Bones[i].Transform;
        }
        return null;
    }
    void DetectCanvasInFront(){
        Ray ray = new Ray();
        
        RaycastHit[] hits;
        hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 100);

        if(hits.Length > 0)
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if(hit.transform.tag == "Canvas"){
                    CanvasEntity canvasTemp = hit.transform.parent.GetComponent<CanvasEntity>();
                    if(canvasTemp.IsUnlock){
                        canvasTarget = hit.transform.parent.GetComponent<CanvasEntity>();
                        canvasTarget.ShowBorder(true);
                    }else{
                        canvasTarget = null;
                    }
                }else{
                    if(canvasTarget != null)
                        canvasTarget.ShowBorder(false);

                    if(!isResizing)
                        canvasTarget = null;
                }
            }
        else{
            if(canvasTarget != null)
                canvasTarget.ShowBorder(false);

            if(!isResizing)
                canvasTarget = null;
        }
    }
    void Save(OVRSkeleton target){
        print("save");
        Gesture g = new Gesture();
        g.name = "new Gesture";
        List<Vector3> data = new List<Vector3>();

        foreach (var bone in fingerBone)
        {
            print(bone.Transform.position);
            data.Add(target.transform.InverseTransformPoint(bone.Transform.position));
        }
        g.fingerDatas = data;
        gestures.Add(g);
    }
    Gesture Recognize(OVRSkeleton skeletonTarget){
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;
        fingerBone = new List<OVRBone>(skeletonTarget.Bones);

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBone.Count; i++)
            {
                Vector3 currenData = skeletonTarget.transform.InverseTransformPoint(fingerBone[i].Transform.position);
                float distance = Vector3.Distance(currenData, gesture.fingerDatas[i]);
                if(distance > threshold){
                    isDiscarded = true;
                    break;
                }
                sumDistance += distance;
            }

            if(!isDiscarded && sumDistance < currentMin){
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }
        return currentGesture;
    }
}
