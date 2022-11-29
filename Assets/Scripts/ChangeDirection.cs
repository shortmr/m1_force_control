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
    public GameObject trial;
          
    private bool direction;
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
        // Set direction to dorsiflexion on start up
        direction = true;
        directionText = "DF";

        text = GetComponentInChildren<TMP_Text>();
        text.text = directionText;

        arrowPositionUp = new Vector3(-8f,3.3f,0f);
        arrowPositionDown = new Vector3(-8f,0.7f,0f);
        trialPositionUp = new Vector3(-10.2f,7.46f,0f);
        trialPositionDown = new Vector3(-10.2f,7.43f,0f);
    }

    public void Flip()
    {
        if (direction) {
            direction = false;
            directionText = "PF";
            desiredEffortSine.GetComponent<DesiredEffortSine>().type = false;
            desiredEffortSine.GetComponent<DesiredEffortSine>().DrawFeedback(desiredEffortSine.GetComponent<LineRenderer>());
            actualEffortControl.GetComponent<ActualEffortControl>().type = false;
            actualEffortMVC.GetComponent<ActualEffortMVC>().type = false;
            arrow.GetComponent<Transform>().localPosition = arrowPositionDown;
            arrow.GetComponent<Transform>().localRotation = Quaternion.Euler(new Vector3(0f,0f,angleDown));
            trial.GetComponent<Transform>().localPosition = trialPositionDown;
            trial.GetComponent<Transform>().localRotation = Quaternion.Euler(new Vector3(0f,0f,angleDown));
        }
        else {
            direction = true;
            directionText = "DF";
            desiredEffortSine.GetComponent<DesiredEffortSine>().type = true;
            desiredEffortSine.GetComponent<DesiredEffortSine>().DrawFeedback(desiredEffortSine.GetComponent<LineRenderer>());
            actualEffortControl.GetComponent<ActualEffortControl>().type = true;
            actualEffortMVC.GetComponent<ActualEffortMVC>().type = true;
            arrow.GetComponent<Transform>().localPosition = arrowPositionUp;
            arrow.GetComponent<Transform>().localRotation = Quaternion.Euler(new Vector3(0f,0f,angleUp));
            trial.GetComponent<Transform>().localPosition = trialPositionUp;
            trial.GetComponent<Transform>().localRotation = Quaternion.Euler(new Vector3(0f,0f,angleUp));
        }
        text = GetComponentInChildren<TMP_Text>();
        text.text = directionText;
    }
}
