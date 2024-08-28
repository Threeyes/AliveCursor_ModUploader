using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Threeyes.GameFramework;
using Threeyes.Core;
/// <summary>
/// ��鸽������Ȥ��
/// Todo��
/// -�����ƶ������ϣ�ʱ�̼�⸽��������������Ȥ�㣬����ж���͸��������ҵ���������ȼ���ߵģ��Ǳ��룩������ʹ��Defaultֵ
/// -overrideInterestPointֻ����ʧ��Ż�����л�������ͨ��Сè��������
/// </summary>
public class Cat_ProceduralInterestPointController : MonoBehaviour
{
    public Transform tfDefaultPosEndPoint;//AC�µ�����
    public Transform tfDefaultLookTarget;//AC�µ�����

    //PS����Ϊ�н����ԣ��������ֻ��Ҫֱ����������ֵ����
    public Transform tfPosEndPoint;
    public Transform tfLookTarget;

    //Runtime
    Cat_ProceduralInterestPoint overrideInterestPoint;
    List<Cat_ProceduralInterestPoint> listInterestPoint = new List<Cat_ProceduralInterestPoint>();
    void Update()
    {
        //��overrideInterestPointΪ��ʱ������Ѱ�����µĵ�
        if (overrideInterestPoint == null && listInterestPoint.Count > 0)
        {
            //�����������Ч����
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

    //�ڽ���/�˳�ʱ����Ҫ����
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
