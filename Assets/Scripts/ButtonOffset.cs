using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOffset : MonoBehaviour
{
    public GameObject actualEffortMVC;
    public GameObject actualEffortControl;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Offset()
    {
        actualEffortMVC.GetComponent<ActualEffortMVC>().positionOffset = actualEffortMVC.GetComponent<ActualEffortMVC>().positionEffort_f;
        actualEffortControl.GetComponent<ActualEffortControl>().positionOffset = actualEffortControl.GetComponent<ActualEffortControl>().positionEffort_f;
    }
}
