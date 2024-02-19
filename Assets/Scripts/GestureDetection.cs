using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture{
    public string name;
    public List<Vector3> fingerDatas;
    public List<Quaternion> fingerRot;
    public UnityEvent onRecognized;
}
public class GestureDetection : MonoBehaviour
{
    public float threshold = 0.1f;
    public bool debugMode = true;
    public OVRSkeleton skeletonRight;
    public OVRSkeleton skeletonLeft;
    public List<Gesture> gestures;
    private List<OVRBone> fingerBone;
    private Gesture previousGesture;
    // Start is called before the first frame update
    void Start()
    {
        previousGesture = new Gesture();
    }
    Vector3 thumb,index;
    // Update is called once per frame
    void Update()
    {
        if(debugMode && Input.GetKeyDown(KeyCode.Space)){
            fingerBone = new List<OVRBone>(skeletonRight.Bones);
            Save();
        }else{
            Gesture currentGesture = Recognize();
            bool hasRecognized = !currentGesture.Equals(new Gesture());

            if(hasRecognized && !currentGesture.Equals(previousGesture)){
                print("new Gesture "+currentGesture.name);
                previousGesture = currentGesture;
                // if(currentGesture.name == "Bant*ng"){
                //     passthroughLayer.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor;
                // }else{
                //     passthroughLayer.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment;
                // }
                // currentGesture.onRecognized.Invoke();
                fingerBone = new List<OVRBone>(skeletonRight.Bones);
            }
            for (int i = 0; i < fingerBone.Count; i++)
            {
                if(fingerBone[i].Id == OVRSkeleton.BoneId.Hand_Thumb3) thumb = fingerBone[i].Transform.position;
                if(fingerBone[i].Id == OVRSkeleton.BoneId.Hand_Index2) index = fingerBone[i].Transform.position;
            }
            // if(_mirror && currentGesture.name != "Love"){
            //     foreach (var item in _handMirror.HandPoins)
            //     {
            //         OVRBone bone = fingerBone.Find(x => x.Id == item.id);
            //         if(bone != null)
            //             item.target.localRotation = fingerBone.Find(x => x.Id == item.id).Transform.localRotation;
            //         else{
            //             print("not found "+item.target.name);
            //         }
            //     }
            // }else if(currentGesture.name == "Love"){
            //     ForceGesture(gestures.Find(x => x.name == "ThumbsUp"));
            // }
        }
    }
    void Save(){
        print("save");
        Gesture g = new Gesture();
        g.name = "new Gesture";
        List<Vector3> data = new List<Vector3>();
        List<Quaternion> rotData = new List<Quaternion>();

        foreach (var bone in fingerBone)
        {
            print(bone.Transform.position);
            data.Add(skeletonRight.transform.InverseTransformPoint(bone.Transform.position));
            rotData.Add(bone.Transform.localRotation);
        }
        g.fingerDatas = data;
        g.fingerRot = rotData;
        gestures.Add(g);
    }
    Gesture Recognize(){
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;
        fingerBone = new List<OVRBone>(skeletonRight.Bones);

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBone.Count; i++)
            {
                Vector3 currenData = skeletonRight.transform.InverseTransformPoint(fingerBone[i].Transform.position);
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
