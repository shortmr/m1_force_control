using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartControlTrial : MonoBehaviour
{
    public GameObject target;
    public GameObject actual;
    public GameObject mvc;
    public GameObject arrow;
    public int mode; // MVC or Force Control
    public GameObject restText;

    private int start;
    private string startText;
    private TMP_Text text;


    // Start is called before the first frame update
    void Start()
    {
        // Set mode to force on startup
        start = 1;
        startText = "Start";
        GetComponent<Image>().color = new Color(255f*1f, 255f*0.89f, 255f*0f);
        text = GetComponentInChildren<TMP_Text>();
        text.text = startText;
    }

    public void StartTrial()
    {
        if (start == 1) {
            start = 0;
            startText = "...";
            GetComponent<Image>().color = new Color(255f*1f, 255f*0f, 255f*0f);
            if (mode == 2) {
                // Force Control
                target.GetComponent<DesiredEffortControl>().begin = true;
                actual.GetComponent<ActualEffortControl>().begin = true;
            } else if (mode == 1) {
                // MVC
                mvc.GetComponent<ActualEffortMVC>().begin = true;
                restText.SetActive(false);
                arrow.SetActive(true);
            }
        }
        else if (start == 0)  {
            start = 1;
            startText = "Start";
            GetComponent<Image>().color = new Color(255f*1f, 255f*0.89f, 255f*0f);
            if (mode == 1) {
                // MVC
                restText.SetActive(true);
                arrow.SetActive(false);
            }
        }
        text = GetComponentInChildren<TMP_Text>();
        text.text = startText;
    }
}
