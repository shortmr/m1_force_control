using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI; // Required when Using UI elements.


public class UpdateMVC : MonoBehaviour
{
    public GameObject settings;
    public bool dorsiflexion;
    public GameObject actualEffort;

    private float mvcValue;
    private InputField mvcText;

    public void Start()
    {
        mvcText = GetComponent<InputField>();
        if(float.TryParse(mvcText.text, out mvcValue))
        {
            if (dorsiflexion) {
                settings.GetComponent<ControlSettings>().mvcValueDF = mvcValue;
                actualEffort.GetComponent<ActualEffortControl>().mvcValueDF = mvcValue;
            } else {
                settings.GetComponent<ControlSettings>().mvcValuePF = mvcValue;
                actualEffort.GetComponent<ActualEffortControl>().mvcValuePF = mvcValue;
            }
        }

    }

    // Invoked when the value of the text field changes.
    public void ValueChangeCheck()
    {
        mvcText = GetComponent<InputField>();
        if(float.TryParse(mvcText.text, out mvcValue))
        {
            if (mvcValue == 0.0f) {
                mvcValue = 1.0f; //avoid divide by zero
                mvcText.text = mvcValue.ToString("0.0");
            }
            if (dorsiflexion) {
                settings.GetComponent<ControlSettings>().mvcValueDF = mvcValue;
                actualEffort.GetComponent<ActualEffortControl>().mvcValueDF = mvcValue;
            } else {
                settings.GetComponent<ControlSettings>().mvcValuePF = mvcValue;
                actualEffort.GetComponent<ActualEffortControl>().mvcValuePF = mvcValue;
            }
        }
    }
}