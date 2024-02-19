using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class AlwaysFace : MonoBehaviour
{
    enum Direction
    {
        None, Up, Right, Forward
    }
    [SerializeField]
    Direction _dir;
    [SerializeField]
    bool _useMainCam = false;
    public Transform _target;
    // Start is called before the first frame update
    void Start()
    {
        if(_useMainCam){
            _target = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_target != null){
            if(_dir == Direction.None){
                transform.LookAt(_target);
            }else if(_dir == Direction.Up){
                transform.rotation = Quaternion.LookRotation(_target.position - transform.position, Vector3.up);
            }else if(_dir == Direction.Right){
                transform.rotation = Quaternion.LookRotation(_target.position - transform.position, Vector3.right);
            }else if(_dir == Direction.Forward){
                transform.rotation = Quaternion.LookRotation(_target.position - transform.position, Vector3.forward);
            }

        }
    }
}
