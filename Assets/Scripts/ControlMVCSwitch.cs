using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlMVCSwitch : MonoBehaviour
{
    public GameObject MVC;
    public GameObject forceControl;
    public GameObject stageText;
    public GameObject startTrial;
    public GameObject performanceDisplay;
    public GameObject directionMode;
    public int mode;

    private string modeText;
    private TMP_Text text;
    private TMP_Text feedbackText;

    // Start is called before the first frame update
    void Start()
    {
        // Set mode to force on startup
        mode = 1;
        modeText = "MVC";
        MVC.SetActive(true);
        forceControl.SetActive(false);

        startTrial.GetComponent<StartControlTrial>().mode = mode;
        text = GetComponentInChildren<TMP_Text>();
        text.text = modeText;
    }

    public void Switch()
    {
        stageText.GetComponent<TextMeshProUGUI>().text = "0";
        if (mode == 1) {
            mode = 2;
            modeText = "Force Control";
            forceControl.SetActive(true);
            MVC.SetActive(false);
        }
        else if (mode == 2)  {
            mode = 1;
            modeText = "MVC";
            if (directionMode.GetComponent<ChangeDirection>().direction == 3) {
                directionMode.GetComponent<ChangeDirection>().Flip(); // avoid undefined mode 3 for MVC
            }
            MVC.SetActive(true);
            forceControl.SetActive(false);
            feedbackText = performanceDisplay.GetComponentInChildren<TMP_Text>();
            feedbackText.text = "";
        }
        startTrial.GetComponent<StartControlTrial>().mode = mode;
        text = GetComponentInChildren<TMP_Text>();
        text.text = modeText;
    }
}

