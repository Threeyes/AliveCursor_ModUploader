using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Cat_ProceduralInterestPoint : MonoBehaviour
{
    /// <summary>
    /// Where the body moved to
    /// 
    /// PS:
    /// -一般是投影到平面上的点
    /// </summary>
    public abstract Vector3 EndPointPos { get; }

    /// <summary>
    /// Where the head looked at
    /// </summary>
    public abstract Vector3 LookTargetPos { get; }
}
