using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesiredEffortSine : MonoBehaviour
{
    public GameObject desiredEffort;
    public GameObject actualEffort;
    public GameObject settings;
    public int type;

    private LineRenderer primaryRenderer;
    private bool startup = true;
    private float mvcRange;
    private float mvcOffset;
    private float mvcRangeCombined;
    private float mvcOffsetCombined;
    private float primaryFrequency;
    private float secondaryFrequency;
    private float trialDuration;

    private int screenWidth = 25;
    private float pointCount = 1000f;
    private float gain;


    // Start is called before the first frame update
    void Start()
    {
        primaryRenderer = GetComponent<LineRenderer>();
        DrawFeedback(primaryRenderer);
    }

    void OnEnable() {
        if (startup) {
            type = 1;
            primaryRenderer = GetComponent<LineRenderer>();
            startup = false;
        }
        else {
            DrawFeedback(primaryRenderer);
        }
    }

    void OnDisable() {
        primaryRenderer.positionCount = 0;
    }

    public void DrawFeedback(LineRenderer renderer) {
        // load force control params from settings
        mvcRange = settings.GetComponent<ControlSettings>().mvcRange;
        mvcOffset = settings.GetComponent<ControlSettings>().mvcOffset;
        mvcRangeCombined = settings.GetComponent<ControlSettings>().mvcRangeCombined;
        mvcOffsetCombined = settings.GetComponent<ControlSettings>().mvcOffsetCombined;
        primaryFrequency = settings.GetComponent<ControlSettings>().primaryFrequency;
        secondaryFrequency = settings.GetComponent<ControlSettings>().secondaryFrequency;
        trialDuration = settings.GetComponent<ControlSettings>().trialDuration;
        gain = settings.GetComponent<ControlSettings>().gain;

        if (secondaryFrequency > 0.0f) {
            // period of sum of sines is LCM of two periods
            if (primaryFrequency <= secondaryFrequency) {
                renderer.positionCount = (int) ((float) pointCount*trialDuration*primaryFrequency);
            } else {
                renderer.positionCount = (int) ((float) pointCount*trialDuration*secondaryFrequency);
            }
        } else {
            renderer.positionCount = (int) ((float) pointCount*trialDuration*primaryFrequency);
        }

        for (int i = 0; i < renderer.positionCount; i++)
        {
            float x = screenWidth * ((float) i / (float) renderer.positionCount - 0.5f);
            float sin = 0f;
            if (secondaryFrequency > 0.0f) {
                float sin_p = 0.5f*Mathf.Sin(2f * Mathf.PI * primaryFrequency * trialDuration * (float) i / (float) renderer.positionCount);
                float sin_s = 0.5f*Mathf.Sin(2f * Mathf.PI * secondaryFrequency * trialDuration * (float) i / (float) renderer.positionCount);
                sin = sin_p + sin_s; // sum of sines
            } else {
                sin = Mathf.Sin(2f * Mathf.PI * primaryFrequency * trialDuration * (float) i / (float) renderer.positionCount);
            }
            float y;
            if (type == 1) {
                // dorsiflexion
                y = mvcRange * sin + mvcOffset;
                renderer.SetPosition(i, new Vector3(x, gain * y + 2f, 0f));
            } else if (type == 2) {
                // plantarflexion
                y = mvcRange * sin + mvcOffset;
                renderer.SetPosition(i, new Vector3(x, -1 * gain * y + 2f, 0f));
            } else if (type == 3) {
                // dorsiflexion + plantarflexion
                y = mvcRangeCombined * sin + mvcOffsetCombined;
                renderer.SetPosition(i, new Vector3(x, gain * y + 2f, 0f));
            }
        }
        desiredEffort.GetComponent<DesiredEffortControl>().ready = true;
        actualEffort.GetComponent<ActualEffortControl>().ready = true;
    }
}
