using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class FilterMenu : MonoBehaviour, IGameObjectFilter
{
    public bool Filter(GameObject gameObject)
    {
        return !MenuScript.Instance.Visible;
    }
}
