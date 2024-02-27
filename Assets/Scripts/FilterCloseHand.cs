using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public class FilterCloseHand : MonoBehaviour, IGameObjectFilter
{
    [SerializeField]
    float distance = 1;
    OVRHand[] hands;
    void Start(){
        hands = GameObject.FindObjectsByType<OVRHand>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
    }
    public bool Filter(GameObject gameObject)
    {
        for (int i = 0; i < hands.Length; i++)
        {
            if(Vector3.Distance(hands[i].transform.position,transform.position) < distance)
                return false;
        }
        return true;
    }
}
