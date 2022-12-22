using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlSettings : MonoBehaviour
{
    public float frameRate;
    public string m1; // string (m1_x or m1_y)
    public float mvcValuePF; // Nm (from torque sensor)
    public float mvcValueDF; // Nm (from torque sensor)
    public float mvcRange; // % MVC (uni-directional)
    public float mvcOffset; // % MVC (uni-directional)
    public float mvcRangeCombined; // % MVC (bi-directional DF and PF)
    public float mvcOffsetCombined; // % MVC (bi-directional DF and PF)
    public float primaryFrequency; // Hz
    public float secondaryFrequency; // Hz
    public float trialDuration; // seconds
    public float mvcDuration; // seconds
    public float gain; // scale factor
    public float filterCutoff; // Hz

    public GameObject arrow;
    public GameObject trial;
    public GameObject stageText;
    public GameObject m1Text;

    void Start()
    {
        Application.targetFrameRate = (int) frameRate;
        QualitySettings.vSyncCount = 0;

        stageText.GetComponent<TextMeshProUGUI>().text = "0";
        m1Text.GetComponent<TextMeshProUGUI>().text = m1;

        // Initialize pre-trial arrows
        arrow.SetActive(false);
        trial.SetActive(true);
    }
}