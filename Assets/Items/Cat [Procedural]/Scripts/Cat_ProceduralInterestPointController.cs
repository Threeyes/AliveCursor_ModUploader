using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Threeyes.GameFramework;
using Threeyes.Core;
/// <summary>
/// 检查附近的兴趣点
/// Todo：
/// -挂载移动物体上，时刻检测附近有无新增的兴趣点，如果有多个就根据条件找到最近或优先级最高的（非必须），否则使用Default值
/// -overrideInterestPoint只有消失后才会进行切换，可以通过小猫触碰销毁
/// </summary>
public class Cat_ProceduralInterestPointController : MonoBehaviour
{
    public Transform tfDefaultPosEndPoint;//AC下的物体
    public Transform tfDefaultLookTarget;//AC下的物体

    //PS：作为中介属性，其他组件只需要直接引用以下值即可
    public Transform tfPosEndPoint;
    public Transform tfLookTarget;

    //Runtime
    Cat_ProceduralInterestPoint overrideInterestPoint;
    List<Cat_ProceduralInterestPoint> listInterestPoint = new List<Cat_ProceduralInterestPoint>();
    void Update()
    {
        //当overrideInterestPoint为空时，尝试寻找最新的点
        if (overrideInterestPoint == null && listInterestPoint.Count > 0)
        {
            //查找最近的有效物体
            Cat_ProceduralInterestPoint cloestInterestPoint = null;
            for (int i = 0; i != listInterestPoint.Count; i++)
            {
                Cat_ProceduralInterestPoint interestPoint = listInterestPoint[i];
                if (interestPoint == null)
                    continue;

                if (cloestInterestPoint == null)
                    cloestInterestPoint = interestPoint;
                else if (Vector3.Distance(transform.position, interestPoint.LookTargetPos) < Vector3.Distance(transform.position, cloestInterestPoint.LookTargetPos))
                    cloestInterestPoint = interestPoint;
            }
            overrideInterestPoint = cloestInterestPoint;
        }

        tfPosEndPoint.position = overrideInterestPoint != null ? overrideInterestPoint.EndPointPos : tfDefaultPosEndPoint.position;
        tfLookTarget.position = overrideInterestPoint != null ? overrideInterestPoint.EndPointPos : tfDefaultLookTarget.position;
    }

    //在进入/退出时都需要更新
    private void OnTriggerEnter(Collider otherCol)
    {
        if (otherCol && otherCol.attachedRigidbody)
        {
            Cat_ProceduralInterestPoint interestPoint = otherCol.attachedRigidbody.GetComponent<Cat_ProceduralInterestPoint>();
            if (interestPoint != null)
                listInterestPoint.AddOnce(interestPoint);
        }
        listInterestPoint.RemoveAll(i => i == null);
    }
    private void OnTriggerExit(Collider otherCol)
    {
        if (otherCol && otherCol.attachedRigidbody)
        {
            Cat_ProceduralInterestPoint interestPoint = otherCol.attachedRigidbody.GetComponent<Cat_ProceduralInterestPoint>();
            if (interestPoint != null)
            {
                if (overrideInterestPoint == interestPoint)
                    overrideInterestPoint = null;
                listInterestPoint.Remove(interestPoint);

            }
        }
    }
}
