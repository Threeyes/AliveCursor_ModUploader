using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonumentValley_UnitController : MonoBehaviour
{
    public Vector3 initLocalPos;
    public Vector3 offsetAxis = Vector3.up;//valid axis to offset
    float maxOffsetDistance = 1;//

    [ContextMenu("SaveInitLocalPos")]
    public void SaveInitLocalPos()
    {
        initLocalPos = transform.localPosition;
    }

    public void Offset(float percent)
    {
        transform.localPosition = initLocalPos + offsetAxis * percent * maxOffsetDistance;
    }
}
