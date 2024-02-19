using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    bool withRotation = false, mirrorY = false;
    [SerializeField]
    Vector3 offset = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(target != null){
            transform.position = target.position+offset;
        }
        if(withRotation && mirrorY){
            transform.localRotation = new Quaternion(-target.localRotation.x,
                target.localRotation.y,
                target.localRotation.z,
                -target.localRotation.w
            );
        }else if(withRotation){
            transform.rotation = target.rotation;
        }
    }
}
