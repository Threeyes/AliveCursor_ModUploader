using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Threeyes.Coroutine;
/// <summary>
/// 
/// Todo:
/// -���ƺ����ϵ�SDK��
/// 
/// ���ܣ�
/// -������ģ�͵�λ��/��ת
/// </summary>
public class CreeperGhostController : MonoBehaviour
{
    [Header("Body")]
    public Transform tfModelRoot;//ģ�����ɣ������壩
    public float bodyMoveSpeed = 5;
    public float bodyRotateSpeed = 1;
    public float maxBodyTurn = 90;//���������תֵ
    public Transform tfGhostBody;//���ɵ�Ŀ��㣬���������λ�Ƽ���ת������ʹ��һ��������Ƶĺô��ǣ������ɵ��޸Ĳ���Ӱ�쵽�ţ������ĸ������λ�á���ת��ʵ����Ծ�����¡�ת��ȶ�����

    [Header("Legs")]
    //ToUpdate����װΪ�����࣬���������չ
    //PS:����Ҫ���飨�����϶����£���ÿ��ֻ���ƶ�һ��ţ���;��Ϯʱ����Ž����ƶ�������������������ߡ�
    public List<CreeperLegGhostController> listLegController1 = new List<CreeperLegGhostController>();
    public List<CreeperLegGhostController> listLegController2 = new List<CreeperLegGhostController>();
    public float moveLegIntervalTime = 0.2f;
    [Header("Runtime")]
    public bool isGroup1Moving = false;
    public bool isGroup2Moving = false;

    public int lastMoveGroupIndex = -1;
    public float lastMoveTime = 0;

    private void Update()
    {
        //# Body
        //��ģ�͸�����(����)����Ghostͬ��

        //����Ӧ���Ƕ�̬���ݽŵ�λ�ü����
        Vector3 centerPos = Vector3.zero;
        listLegController1.ForEach(com => centerPos += com.tfSourceTarget.position);
        listLegController2.ForEach(com => centerPos += com.tfSourceTarget.position);
        centerPos /= (listLegController1.Count + listLegController2.Count);
        centerPos += tfGhostBody.localPosition;//������겻��Ҫ��������ֵ
        tfModelRoot.position = Vector3.Lerp(tfModelRoot.position, centerPos, Time.deltaTime * bodyMoveSpeed);// tfGhostBody.position;

        //ͨ��tfGhostBody�������ɵ���ת
        //Todo:���������תֵ
        Quaternion targetRotation = tfGhostBody.rotation;
        //tfModelRoot.rotation = Quaternion.Lerp(tfModelRoot.rotation, targetRotation, Time.deltaTime * bodyRotateSpeed);
        tfModelRoot.rotation = targetRotation;

        //# Legs
        ///-�����һ����Ҫ����λ����ƫ�����������Ǿ��ȸ��¸��飻ͬʱֻ����һ������ƶ�
        bool shouldGroup1Move = listLegController1.Any(com => com.NeedMove);
        bool shouldGroup2Move = listLegController2.Any(com => com.NeedMove);
        if (shouldGroup1Move && shouldGroup2Move)
        {
            ///-��¼�ϴ��ƶ����飬����Ѿ��ƶ����ұ���2�鶼��Ҫ�ƶ�������һ����޷������ƶ�����Ϊ��һ�����ƶ�
            int curMoveIndex = lastMoveGroupIndex == 1 ? 2 : 1;
            LegGroupTweenMove(curMoveIndex);
        }
        if (shouldGroup1Move)
        {
            LegGroupTweenMove(1);
        }
        else if (shouldGroup2Move)
        {
            LegGroupTweenMove(2);
        }
    }

    public void LegGroupForceTweenMove()
    {
        LegGroupTweenMove(0, true);
        LegGroupTweenMove(1, true);
    }
    void LegGroupTweenMove(int index, bool forceUpdate = false)
    {
        if (Time.time - lastMoveTime < moveLegIntervalTime)//�����ƶ�֮��Ҫ�м��������ܼ�
        {
            return;
        }
        var listTarget = (index == 1 ? listLegController1 : listLegController2);
        listTarget.ForEach(com => com.TweenMoveAsync(forceUpdate));
        lastMoveGroupIndex = index;
        lastMoveTime = Time.time;
    }

    [Header("Editor")]
    public float gizmosRadius = 0.1f;
    private void OnDrawGizmos()
    {
        if (tfGhostBody)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(tfGhostBody.position, tfGhostBody.position + tfGhostBody.right * gizmosRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(tfGhostBody.position, tfGhostBody.position + tfGhostBody.up * gizmosRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(tfGhostBody.position, tfGhostBody.position + tfGhostBody.forward * gizmosRadius);
        }
    }
}
