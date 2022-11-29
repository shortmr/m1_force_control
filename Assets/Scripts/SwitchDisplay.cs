using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwitchDisplay : MonoBehaviour
{
    public GameObject cameraM1;

    private int display;
    private string displayText;
    private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        displayText = PlayerPrefs.GetString("display", "Display 2"); // get stored display
        if (displayText == "Display 2") {
            cameraM1.GetComponent<Camera>().targetDisplay = 1;
        }
        else {
            cameraM1.GetComponent<Camera>().targetDisplay = 2;
        }

        display = cameraM1.GetComponent<Camera>().targetDisplay;

        text = GetComponentInChildren<TMP_Text>();
        text.text = displayText;
    }

    public void Switch()
    {
        if (display == 1) {
            cameraM1.GetComponent<Camera>().targetDisplay = 2;
            display = 2;
            displayText = "Display 3";
        }
        else {
            cameraM1.GetComponent<Camera>().targetDisplay = 1;
            display = 1;
            displayText = "Display 2";
        }

        text = GetComponentInChildren<TMP_Text>();
        text.text = displayText;

        PlayerPrefs.SetString("display",displayText);
    }
}
