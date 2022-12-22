using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeDirection : MonoBehaviour
{
    public GameObject desiredEffortSine;
    public GameObject actualEffortControl;
    public GameObject actualEffortMVC;
    public GameObject arrow;
    public GameObject trialCombined;
    public GameObject trial;
    public GameObject effortMode;
    public int direction;

    private int mode;
    private string directionText;
    private TMP_Text text;
    private Vector3 arrowPositionUp;
    private Vector3 arrowPositionDown;   
    private Vector3 trialPositionUp;
    private Vector3 trialPositionDown;
    private float angleUp = 0f;
    private float angleDown = 180f;

    // Start is called before the first frame update
    void Start()
    {
        // Set direction to dorsiflexion on start up (up: 1, down: 2, combined: 3)
        direction = 1;
        directionText = "DF";

        text = GetComponentInChildren<TMP_Text>();
        text.text = directionText;

        mode = effortMode.GetComponent<ControlMVCSwitch>().mode;

        arrowPositionUp = new Vector3(-8f,3.3f,0f);
        arrowPositionDown = new Vector3(-8f,0.7f,0f);
        trialPositionUp = new Vector3(-10.2f,7.46f,0f);
        trialPositionDown = new Vector3(-10.2f,7.43f,0f);
        trialCombined.SetActive(false);
    }

    public void Flip()
    {
        // Check if in MVC or force control mode
        mode = effortMode.GetComponent<ControlMVCSwitch>().mode;
        if (mode == 1) {
            if (direction == 1) {
                direction = 2;
            } else if (direction == 2) {
                direction = 1;
            } else if (direction == 3) {
                direction = 1; // direction option 3 is not defined for MVC trials
            }
        } else if (mode == 2) {
            if (direction == 1) {
                direction = 2;
            } else if (direction == 2) {
                direction = 3;
            } else if (direction == 3) {
                direction = 1;
            }
        }
         if (direction == 1) {
            directionText = "DF";
            desiredEffortSine.GetComponent<DesiredEffortSine>().type = 1;
            desiredEffortSine.GetComponent<DesiredEffortSine>().DrawFeedback(desiredEffortSine.GetComponent<LineRenderer>());
            actualEffortControl.GetComponent<ActualEffortControl>().type = 1;
            actualEffortMVC.GetComponent<ActualEffortMVC>().type = 1;
            arrow.GetComponent<Transform>().localPosition = arrowPositionUp;
            arrow.GetComponent<Transform>().localRotation = Quaternion.Euler(new Vector3(0f,0f,angleUp));
            trial.GetComponent<Transform>().localPosition = trialPositionUp;
            trial.GetComponent<Transform>().localRotation = Quaternion.Euler(new Vector3(0f,0f,angleUp));
            trialCombined.SetActive(false);
        } else if (direction == 2) {
            directionText = "PF";
            desiredEffortSine.GetComponent<DesiredEffortSine>().type = 2;
            desiredEffortSine.GetComponent<DesiredEffortSine>().DrawFeedback(desiredEffortSine.GetComponent<LineRenderer>());
            actualEffortControl.GetComponent<ActualEffortControl>().type = 2;
            actualEffortMVC.GetComponent<ActualEffortMVC>().type = 2;
            arrow.GetComponent<Transform>().localPosition = arrowPositionDown;
            arrow.GetComponent<Transform>().localRotation = Quaternion.Euler(new Vector3(0f,0f,angleDown));
            trial.GetComponent<Transform>().localPosition = trialPositionDown;
            trial.GetComponent<Transform>().localRotation = Quaternion.Euler(new Vector3(0f,0f,angleDown));
            trialCombined.SetActive(false);
        } else if (direction == 3) {
            directionText = "DF/PF";
            desiredEffortSine.GetComponent<DesiredEffortSine>().type = 3;
            desiredEffortSine.GetComponent<DesiredEffortSine>().DrawFeedback(desiredEffortSine.GetComponent<LineRenderer>());
            actualEffortControl.GetComponent<ActualEffortControl>().type = 3;
            trial.GetComponent<Transform>().localPosition = trialPositionUp;
            trial.GetComponent<Transform>().localRotation = Quaternion.Euler(new Vector3(0f,0f,angleUp));
            trialCombined.SetActive(true);
        }
        text = GetComponentInChildren<TMP_Text>();
        text.text = directionText;
    }
}
