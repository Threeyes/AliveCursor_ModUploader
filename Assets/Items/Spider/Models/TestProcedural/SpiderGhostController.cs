using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// 
/// Todo:
/// -ȡ��Ϊ��ͨ�ã������棬Ghost��ΪGuider
/// -���ƺ����ϵ�SDK��
/// 
/// ���ܣ�
/// -������ģ�͵�λ��/��ת
/// </summary>
public class SpiderGhostController : MonoBehaviour
{
    public Transform tfModelRoot;//ģ�͸�����(����)

    [Header("Body")]
    public float bodyMoveSpeed = 5;
    public Vector3 bodyOffset = Vector3.zero;

    public Transform tfGhostBody;//���������������ת����Ӱ��ŵ���ת��λ�á�����Ҫ��������ΧΪ���ڲ���

    [Header("Leg")]
    //ToUpdate����װΪ�����࣬���������չ
    //PS:����Ҫ���飨�����϶����£���ÿ��ֻ�ܸ���һ��š����������ŵ����ߡ�
    public List<SpiderLegController> listLegController1 = new List<SpiderLegController>();
    public List<SpiderLegController> listLegController2 = new List<SpiderLegController>();
    public float moveLegIntervalTime = 0.2f;
    [Header("Runtime")]
    public bool isGroup1Moving = false;
    public bool isGroup2Moving = false;

    public int lastMoveGroupIndex = -1;
    public float lastMoveTime = 0;
    private void Update()
    {
        //��ģ�͸�����(����)����Ghostͬ��

        //����Ӧ���Ƕ�̬���ݽŵ�λ�ü����
        Vector3 centerPos = Vector3.zero;
        listLegController1.ForEach(com => centerPos += com.tfSourceTarget.position);
        listLegController2.ForEach(com => centerPos += com.tfSourceTarget.position);
        centerPos /= (listLegController1.Count + listLegController2.Count);
        tfModelRoot.position = Vector3.Lerp(tfModelRoot.position, centerPos, Time.deltaTime * bodyMoveSpeed) + bodyOffset;// tfGhostBody.position;

        //ͨ��tfGhostBody�������ɵ���ת
        tfModelRoot.rotation = tfGhostBody.rotation;

        ///Todo:
        ///-�����һ����Ҫ����λ����ƫ�����������Ǿ��ȸ��¸��飻ͬʱֻ����һ������ƶ�
        ///-���ƶ�������ΪAsync������������
        bool shouldGroup1Move = listLegController1.Any(com => com.NeedMove);
        bool shouldGroup2Move = listLegController2.Any(com => com.NeedMove);

        if (shouldGroup1Move && shouldGroup2Move)
        {
            ///-��¼�ϴ��ƶ����飬����Ѿ��ƶ����ұ���2�鶼��Ҫ�ƶ�������һ����޷������ƶ�����Ϊ��һ�����ƶ�
            int curMoveIndex = lastMoveGroupIndex == 1 ? 2 : 1;
            MoveGroup(curMoveIndex);
        }
        if (shouldGroup1Move)
        {
            MoveGroup(1);
        }
        else if (shouldGroup2Move)
        {
            MoveGroup(2);
        }
    }

    void MoveGroup(int index)
    {
        if (Time.time - lastMoveTime < moveLegIntervalTime)//�����ƶ�֮��Ҫ�м��������ܼ�
        {
            return;
        }
        var listTarget = (index == 1 ? listLegController1 : listLegController2);
        listTarget.ForEach(com => com.TweenMoveAsync());
        lastMoveGroupIndex = index;
        lastMoveTime = Time.time;
    }
}
