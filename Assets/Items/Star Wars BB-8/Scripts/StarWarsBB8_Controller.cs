using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarWarsBB8_Controller : MonoBehaviour
{
    public Transform tfHeadRig;

    public Vector2 rotateHeadIntervalRange = new Vector2(1, 5);
    public float rotateSpeed = 1;
    //Runtime
    Vector3 initLocalEulerAngles;
    float lastRotateHeadTime = 0;
    float nextRotateHeadTimeInterval = 1;
    float nextRotateAngle = 0;
    private void Start()
    {
        initLocalEulerAngles = tfHeadRig.localEulerAngles;
    }
    private void Update()
    {
        if (Time.time - lastRotateHeadTime > nextRotateHeadTimeInterval)
        {
            nextRotateAngle = Random.Range(0, 360);
            nextRotateHeadTimeInterval = Random.Range(rotateHeadIntervalRange.x, rotateHeadIntervalRange.y);
            lastRotateHeadTime = Time.time;
        }
        initLocalEulerAngles.z = Mathf.Lerp(initLocalEulerAngles.z, nextRotateAngle, rotateSpeed * Time.deltaTime);
        tfHeadRig.localEulerAngles = initLocalEulerAngles;
    }
}
