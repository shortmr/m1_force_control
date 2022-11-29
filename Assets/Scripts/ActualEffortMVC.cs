using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI; // Required when Using UI elements.

public class ActualEffortMVC : MonoBehaviour
{
    public GameObject settings;
    public GameObject jointState;
    public GameObject dataField;
    public GameObject stageText;
    public GameObject controlStartButton;
    public GameObject restText;
    public GameObject arrow;
    public GameObject mvcFeedback;

    public float positionEffort;
    public float positionEffort_f;
    public float positionOffset;
    public GameObject mvcValueDF;
    public GameObject mvcValuePF;

    public bool ready;
    public bool begin;
    public bool type;

    private bool pass;
    private bool rest;
    private GameObject data;
    private float mvcDuration;
    private float previousEffort_f = 0.0f;
    private float gain;
    private List<float> counterList = new List<float>();
    private List<float> actualEffortList = new List<float>();
    private float counter;
    private float alpha_effort;
    private float cut_off_;
    private float frameRate;

    // Start is called before the first frame update
    void Start()
    {
        data = jointState;
        gain = settings.GetComponent<ControlSettings>().gain;
        mvcDuration = settings.GetComponent<ControlSettings>().mvcDuration;
        cut_off_ = settings.GetComponent<ControlSettings>().filterCutoff;
        frameRate = settings.GetComponent<ControlSettings>().frameRate;

        restText.SetActive(true);

        ready = true;
        pass = false;
        begin = false;
        rest = true;
        type = true;
        counter = 0f;

        // Discrete time filter for torque plot
        float dt = 1f/frameRate;
        alpha_effort = (2f * Mathf.PI * dt * cut_off_)/(2f * Mathf.PI * dt * cut_off_ + 1f);
    }

    void OnEnable() {
        ready = true;
        rest = true;
        restText.SetActive(true);
    }

    void OnDisable() {
        restText.SetActive(false);
        arrow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // flip sign of interaction torque
        positionEffort = -1*data.GetComponent<JointSubscriber>().tau_s;

        // filter torque
        positionEffort_f = alpha_effort * positionEffort + (1f - alpha_effort) * previousEffort_f;

        if (ready) {
            ready = false;
            pass = true;
            counter = 0f;
            counterList.Clear();
            actualEffortList.Clear();
        }
        if (pass & begin) {
            counter += Time.deltaTime; //1f/(float) Application.targetFrameRate;
            if (counter >= mvcDuration + 3f) {
                ready = true;
                pass = false;
                rest = true;
                begin = false;
                controlStartButton.GetComponent<StartControlTrial>().StartTrial();

                var sb = new StringBuilder("Time,Desired,Actual");
                for (int i = 0; i < (int) actualEffortList.Count; i++)
                {
                    float time = counterList[i];
                    float y = actualEffortList[i];
                    int target = 0;
                    if (type) {
                        target = 1;
                    }
                    sb.Append('\n').Append(time.ToString()).Append(',').Append(target.ToString()).Append(',').Append(y.ToString());
                }
                SaveToCSV(sb.ToString());

                int stageCount;
                int.TryParse(stageText.GetComponent<TextMeshProUGUI>().text, out stageCount);
                stageCount += 1;
                stageText.GetComponent<TextMeshProUGUI>().text = stageCount.ToString();
                // get maximum values and update text field
                if (type) {
                    float abs_max_eff = Mathf.Abs(actualEffortList.Max());
                    mvcValueDF.GetComponent<InputField>().text = abs_max_eff.ToString("0.0000");
                } else {
                    float abs_max_eff = Mathf.Abs(actualEffortList.Min());
                    mvcValuePF.GetComponent<InputField>().text = abs_max_eff.ToString("0.0000");
                }
                plotMVCFeedback(actualEffortList);
            } else {
                if (rest & counter >= mvcDuration) {
                    rest = false;
                    arrow.SetActive(false);
                    restText.SetActive(true);
                }
                counterList.Add(counter);
                actualEffortList.Add(positionEffort_f);
            }
        }
        transform.localPosition = new Vector3(0f, gain*(positionEffort_f-positionOffset)+2f, 0.0f);
        previousEffort_f = positionEffort_f;
    }
    public void SaveToCSV (string data)
    {
        // Specify target file path
        var folder = Application.persistentDataPath;
        int trialNumber;
        int.TryParse(stageText.GetComponent<TextMeshProUGUI>().text, out trialNumber);
        string dataText = dataField.GetComponent<InputField>().text;
        string trialType = "";
        if (type) {
            trialType = "df";
        } else {
            trialType = "pf";
        }
        var filePath = Path.Combine(folder, dataText + "_mvc_" + trialType + "_" + trialNumber.ToString("00") + ".csv");

        using(var writer = new StreamWriter(filePath, false))
        {
            writer.Write(data);
        }

        Debug.Log($"CSV file written to \"{filePath}\"");
    }

    public void plotMVCFeedback (List<float> data)
    {
        LineRenderer primaryRenderer = mvcFeedback.GetComponent<LineRenderer>();
        primaryRenderer.positionCount = (int) (data.Count);
        for (int i = 0; i < primaryRenderer.positionCount; i++)
        {
            float x = -11.5f + 4f * ((float) i / (float) primaryRenderer.positionCount); // fixed values
            float y = data[i];
            primaryRenderer.SetPosition(i, new Vector3(x, 1.5f*y/60f, 0f));
        }
    }
}