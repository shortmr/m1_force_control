using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI; // Required when Using UI elements.

public class ActualEffortControl : MonoBehaviour
{
    public GameObject settings;
    public GameObject jointState;
    public GameObject performanceDisplay;
    public GameObject dataField;
    public GameObject stageText;

    public float positionEffort;
    public float positionEffort_f;
    public float positionOffset;
    public GameObject desiredEffort;
    public bool ready;
    public bool begin;
    public int type;

    public float mvcValuePF;
    public float mvcValueDF;

    private GameObject data;
    private TMP_Text feedbackText;
    private float gain;
    private float previousEffort_f = 0.0f;
    private float mvcValue = 1.0f;
    private float trialIndex = 0f;
    private List<float> counterList = new List<float>();
    private List<float> actualEffortList = new List<float>();
    private List<float> desiredEffortList = new List<float>();
    private int screenWidth = 25;

    private bool pass;

    private float trialDuration;
    private bool running;
    private float counter;
    private LineRenderer desiredEffortComponent;
    private Vector3[] effortVec;
    private float alpha_effort;
    private float cut_off_;
    private float frameRate;

    // Start is called before the first frame update
    void Start()
    {
        data = jointState;
        gain = settings.GetComponent<ControlSettings>().gain;
        mvcValueDF = settings.GetComponent<ControlSettings>().mvcValueDF; //TODO: update so that this is refreshed after new values are obtained
        mvcValuePF = settings.GetComponent<ControlSettings>().mvcValuePF;
        trialDuration = settings.GetComponent<ControlSettings>().trialDuration;
        cut_off_ = settings.GetComponent<ControlSettings>().filterCutoff;
        frameRate = settings.GetComponent<ControlSettings>().frameRate;

        running = true;
        ready = false;
        pass = false;
        begin = false;
        type = 1;
        counter = 0f;

        effortVec = new Vector3[1];
        effortVec[0] = new Vector3(0f,0f,0f);

        feedbackText = performanceDisplay.GetComponentInChildren<TMP_Text>();
        feedbackText.text = "";

        // Discrete time filter for torque plot
        float dt = 1f/frameRate;
        alpha_effort = (2f * Mathf.PI * dt * cut_off_)/(2f * Mathf.PI * dt * cut_off_ + 1f);
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
        // flip sign of interaction torque
        positionEffort = -1*data.GetComponent<JointSubscriber>().tau_s;

        // filter torque
        positionEffort_f = alpha_effort * positionEffort + (1f - alpha_effort) * previousEffort_f;

        if (running) {
            if (ready) {
                desiredEffortComponent = desiredEffort.GetComponent<LineRenderer>();
                effortVec = new Vector3[desiredEffortComponent.positionCount];
                desiredEffortComponent.GetPositions(effortVec);
                ready = false;
                pass = true;
                counter = 0f;
//                 if (type == 1) {
//                     //dorsiflexion
//                     mvcValue = mvcValueDF;
//                 } else if (type == 2) {
//                     //plantarflexion
//                     mvcValue = mvcValuePF;
//                 }
                trialIndex = (float) desiredEffortComponent.positionCount * counter/trialDuration;
                counterList.Clear();
                actualEffortList.Clear();
                desiredEffortList.Clear();
            }
            if (pass & begin) {
                GetComponent<LineRenderer>().positionCount = 0;
                counter += Time.deltaTime; //1f/(float) Application.targetFrameRate;
                trialIndex = (float) desiredEffortComponent.positionCount * counter/trialDuration;

                if ((int) trialIndex >= desiredEffortComponent.positionCount-1) {
                    ready = true;
                    pass = false;
                    begin = false;
                    trialIndex = 0f;
                    DrawFeedback(GetComponent<LineRenderer>());
                    int stageCount;
                    int.TryParse(stageText.GetComponent<TextMeshProUGUI>().text, out stageCount);
                    stageCount += 1;
                    stageText.GetComponent<TextMeshProUGUI>().text = stageCount.ToString();
                } else {
                    counterList.Add(counter);
                    actualEffortList.Add(positionEffort_f);
                    desiredEffortList.Add(effortVec[(int)trialIndex].y);
                }
            }
            if ((positionEffort_f-positionOffset) > 0f) {
                //dorsiflexion
                mvcValue = mvcValueDF;
            } else {
                //plantarflexion
                mvcValue = mvcValuePF;
            }
            transform.localPosition = new Vector3(effortVec[(int)trialIndex].x, 100f*gain*(positionEffort_f-positionOffset)/mvcValue+2f, 0.0f);
            previousEffort_f = positionEffort_f;
        }
    }

    public void DrawFeedback(LineRenderer renderer) {
        renderer.positionCount = (int) (actualEffortList.Count);
        float total_err = 0f; // compute total error for rmse
        float total_eff = 0f;
        float avg_eff = actualEffortList.Average();
        var sb = new StringBuilder("Time,Desired,Actual");
        for (int i = 0; i < renderer.positionCount; i++)
        {
            float time = counterList[i];
            float x = screenWidth * ((float) i / (float) renderer.positionCount - 0.5f);
            float y = actualEffortList[i];
            float target = desiredEffortList[i];

            total_err += Mathf.Pow(y-target, 2f);
            total_eff += Mathf.Pow(y-avg_eff, 2f);
            renderer.SetPosition(i, new Vector3(x, 100f*gain*(y-positionOffset)/mvcValue+2f, 0f));

            sb.Append('\n').Append(time.ToString()).Append(',').Append(target.ToString()).Append(',').Append(y.ToString());
        }
        float rmse = Mathf.Sqrt(total_err/(float) renderer.positionCount);
        float sd = Mathf.Sqrt(total_eff/(float) renderer.positionCount); // TODO: filter out movement frequency (Lodha, 2022) or use SPARC
        // update text feedback
        feedbackText.text = "Accuracy = " + rmse.ToString("0.00") + "\nSteadiness = " + sd.ToString("0.00"); // TODO: normalize score values to non-paretic limb
        SaveToCSV(sb.ToString());
    }


    public void SaveToCSV (string data)
    {
        // Specify target file path
        var folder = Application.persistentDataPath;
        int trialNumber;
        int.TryParse(stageText.GetComponent<TextMeshProUGUI>().text, out trialNumber);
        string dataText = dataField.GetComponent<InputField>().text;
        string trialType = "";
        if (type == 1) {
            trialType = "df";
        } else if (type == 2) {
            trialType = "pf";
        } else if (type == 3) {
            trialType = "dfpf";
        }
        var filePath = Path.Combine(folder, dataText + "_control_" + trialType + "_" + trialNumber.ToString("00") + ".csv");

        using(var writer = new StreamWriter(filePath, false))
        {
            writer.Write(data);
        }

        Debug.Log($"CSV file written to \"{filePath}\"");
    }
}
