using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckSliders : MonoBehaviour
{
    public GameObject valueText;
    public bool primaryCheck;
    public GameObject settings;
    public GameObject desiredEffortSine;

    private Slider frequencySlider;
    private float t_;
    private float maxAmount = 0.2f;
    private int numberOfSteps = 0;
    private float stepAmount = 0.05f;
    private string displayText;

    void Start()
    {
        frequencySlider = GetComponent<Slider>();
        frequencySlider.maxValue = maxAmount;
        numberOfSteps = (int) (frequencySlider.maxValue / stepAmount);
        if (primaryCheck) {
            t_= PlayerPrefs.GetFloat("primaryFrequency",0.2f);
            settings.GetComponent<ControlSettings>().primaryFrequency = frequencySlider.value;
        } else {
            t_ = PlayerPrefs.GetFloat("secondaryFrequency",0.0f);
            settings.GetComponent<ControlSettings>().secondaryFrequency = frequencySlider.value;
        }
        frequencySlider.value = t_;
        valueText.GetComponent<TextMeshProUGUI>().text = frequencySlider.value.ToString("0.00");
    }

    public void ChangeValue()
    {
        float range = (frequencySlider.value / frequencySlider.maxValue) * numberOfSteps;
        int ceil = Mathf.CeilToInt(range);
        if (ceil == 3) {
            ceil = 2; // 0.15 Hz not implemented
        }
        frequencySlider.value = ceil * stepAmount;
        displayText = frequencySlider.value.ToString("0.00");
        valueText.GetComponent<TextMeshProUGUI>().text = displayText;
        if (primaryCheck) {
            PlayerPrefs.SetFloat("primaryFrequency",frequencySlider.value);
            settings.GetComponent<ControlSettings>().primaryFrequency = frequencySlider.value;
        } else {
            PlayerPrefs.GetFloat("secondaryFrequency",frequencySlider.value);
            settings.GetComponent<ControlSettings>().secondaryFrequency = frequencySlider.value;
        }
        desiredEffortSine.GetComponent<DesiredEffortSine>().DrawFeedback(desiredEffortSine.GetComponent<LineRenderer>()); // update target
    }
}
