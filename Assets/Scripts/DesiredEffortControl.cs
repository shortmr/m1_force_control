using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesiredEffortControl : MonoBehaviour
{
    public GameObject settings;
    public float positionEffort;
    public GameObject desiredEffort;
    public GameObject controlStartButton;
    public bool ready;
    public bool begin;

    private float gain;
    private float mvcValuePF;
    private float mvcValueDF;
    private float trialDuration;
    private bool running;
    private float counter;
    private LineRenderer desiredEffortComponent;
    private Vector3[] effortVec;
    private bool pass;

    // Start is called before the first frame update
    void Start()
    {
        gain = settings.GetComponent<ControlSettings>().gain;
        mvcValueDF = settings.GetComponent<ControlSettings>().mvcValueDF;
        mvcValuePF = settings.GetComponent<ControlSettings>().mvcValuePF;
        trialDuration = settings.GetComponent<ControlSettings>().trialDuration;
        running = true;
        ready = false;
        pass = false;
        begin = false;
        counter = 0f;
    }

    void OnEnable() {
        running = true;
    }

    void OnDisable() {
        running = false;
        begin = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (running) {
            if (ready) {
                desiredEffortComponent = desiredEffort.GetComponent<LineRenderer>();
                effortVec = new Vector3[desiredEffortComponent.positionCount];
                desiredEffortComponent.GetPositions(effortVec);
                ready = false;
                pass = true;
                counter = 0f;
                float trialIndex = (float) desiredEffortComponent.positionCount * counter/trialDuration;
                transform.localPosition = effortVec[(int)trialIndex];
            }
            if (pass & begin) {
                counter += Time.deltaTime; //1f/(float) Application.targetFrameRate;
                float trialIndex = (float) desiredEffortComponent.positionCount * counter/trialDuration;
                if ((int) trialIndex >= desiredEffortComponent.positionCount) {
                    ready = true;
                    pass = false;
                    begin = false;
                    controlStartButton.GetComponent<StartControlTrial>().StartTrial();
                } else {
                    transform.localPosition = effortVec[(int)trialIndex];
                }
            }
        }
    }

}