using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using JointState = RosMessageTypes.Sensor.JointStateMsg;

public class JointSubscriber : MonoBehaviour
{
    public GameObject settings;
    public string topic;
    public float tau_s;
    public float q;

    private string m1;

    void Start() {
        m1 = settings.GetComponent<ControlSettings>().m1;
        ROSConnection.GetOrCreateInstance().Subscribe<JointState>(m1 + "/" + topic + "/", StreamData);
    }

    void StreamData(JointState d) {
        tau_s = (float)d.effort[0]; // interaction torque
        q = (float)d.position[0]; // M1 angle
    }

    private void Update()
    {

    }
}
