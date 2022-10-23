using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// ����Ghost��������ƶ�
/// 
/// Todo:
/// -���ƺ����ϵ�SDK��
/// 
/// ���ܣ�
/// -������ģ�͵�λ��/��ת
/// </summary>
public class CreeperGhostController : ComponentGroupBase<CreeperLegGhostController>
{
    public bool IsLegsMoving { get { return ListComp.Any(c => c.isMoving); } }

    [Header("Body")]
    public Transform tfModelRoot;//ģ�����ɣ������壩
    public float bodyMoveSpeed = 5;
    public float bodyRotateSpeed = 0.5f;
    public float maxBodyTurn = 90;//���������תֵ
    public Vector3 bodyOffsetToCenter;//��������ڽ����ĵ�Ĭ��ȫ��λ�ƣ�������ǰͨ�����ò˵���SaveBodyCenterOffset���������ã�
    public Transform tfGhostBody;//���ɵ�Ŀ��㣬���������λ�Ƽ���ת������ʹ��һ��������Ƶĺô��ǣ������ɵ��޸Ĳ���Ӱ�쵽�ţ������ĸ������λ�á���ת��ʵ����Ծ�����¡�ת��ȶ�����

    [Header("Legs")]
    public float moveLegIntervalTime = 0.1f;//Warning��Ҫ��CreeperLegGhostController.tweenDurationֵ�󣬷���ĳ��Leg�������ǰTween��ɶ��ٴ��ƶ����Ӷ�����ĳ����Ƶ���ƶ�������
    public float alignAfterHaltDelayTime = 5;//ͣ�º󣬶�ÿ�ʼ�Ŷ��룬����0��Ч

    //PS:����Ҫ���飨�����϶����£���ÿ��ֻ���ƶ�һ��ţ���;��Ϯʱ����Ž����ƶ�������������������ߡ�
    public List<LegControllerGroup> listLegControllerGroup = new List<LegControllerGroup>();


    [Header("Runtime")]
    public int lastMoveGroupIndex = -1;
    public float lastMoveTime = 0;
    public Vector3 baseBodyPosition;
    public bool hasAligned = false;//�ڱ���ͣ�º��Ƿ��Ѿ�����
    void Start()
    {
        //ToAdd���ڳ���ʼʱ��ʼǰ��¼Ĭ�ϵ�λ�ƣ���Ϊ��Щ���ɲ��������ġ���Hand��
        baseBodyPosition = tfModelRoot.position;
    }

    [ContextMenu("SaveBodyOffsetToCenter")]
    void SaveBodyOffsetToCenter()
    {
        Vector3 legsCenterPos = GetLegsCenterPos();
        bodyOffsetToCenter = tfModelRoot.position - legsCenterPos;
    }

    Vector3 GetLegsCenterPos()
    {
        Vector3 centerPos = Vector3.zero;
        ListComp.ForEach(com => centerPos += com.tfSourceTarget.position);
        centerPos /= ListComp.Count;
        return centerPos;
    }
    private void LateUpdate()
    {
        //# Body
        //1.�����нŵ�����λ�ü���ó����ɵ�Ŀ��λ��
        Vector3 bodyTargetPos = GetLegsCenterPos() + bodyOffsetToCenter * AC_ManagerHolder.CommonSettingManager.CursorSize;
        baseBodyPosition = Vector3.Lerp(baseBodyPosition, bodyTargetPos, Time.deltaTime * bodyMoveSpeed);//���ɵ�Ŀ��λ�ã����ƶ���أ�

        //2.����GhostBody��������ƫ������������Ӱ������λ�ã���Ϊ����Ƶ�ȼ�ʱ��Ӧ��أ���˲�����Lerp��
        Vector3 worldOffset = tfGhostBody.parent.TransformDirection(tfGhostBody.localPosition);//����tfGhostBody�ľֲ�λ�ƣ���ת��Ϊȫ��ʸ��
        worldOffset *= AC_ManagerHolder.CommonSettingManager.CursorSize;//���Թ�����ţ���ΪĿ������ͬ�������ţ�
        tfModelRoot.position = baseBodyPosition + worldOffset;//������겻��Ҫ��������ֵ����ΪGhost��Ŀ�����������һ�£����λ�õ�λҲһ�£���Ƶ��ӦҪ��ʱͬ���� 


        //ͨ��tfGhostBody�������ɵ���ת
        //Todo:���������תֵ
        Quaternion targetRotation = tfGhostBody.rotation;
        //tfModelRoot.rotation = Quaternion.Lerp(tfModelRoot.rotation, targetRotation, Time.deltaTime * bodyRotateSpeed);
        tfModelRoot.rotation = targetRotation;//ֱ��ͬ�������ڼ�ʱ��Ӧ��Ƶ

        //# Legs

        ///-�����һ����Ҫ����λ����ƫ�����������Ǿ��ȸ��¸��飻ͬʱֻ����һ������ƶ�
        float maxGroupDistance = 0;//��¼���������ܾ�������
        int needMoveGroupIndex = -1;
        for (int i = 0; i != listLegControllerGroup.Count; i++)
        {
            if (lastMoveGroupIndex == i)//��ֹͬһ�������ƶ�
                continue;
            var lcg = listLegControllerGroup[i];
            if (lcg.NeedMove && lcg.AverageDistance > maxGroupDistance)
            {
                needMoveGroupIndex = i;
                maxGroupDistance = lcg.AverageDistance;
            }
        }
        if (needMoveGroupIndex >= 0)//�������Ҫ�ƶ�
        {
            //if (!(Time.time - lastMoveTime < moveLegIntervalTime))//�����ƶ�֮��Ҫ�м��������ܼ�
            //    Debug.Log(" " + needMoveGroupIndex + ": " + maxGroupDistance);
            LegGroupTweenMoveNew(needMoveGroupIndex);
        }
        else//���нŶ�����Ҫ�ƶ�
        {
            //��ֹͣ�ƶ�һ��ʱ���ǿ�ƶ�������GhostLegs��λ�ã�����ǿ��֢���ߣ��籾�ˣ����ò��Գ�
            if (!hasAligned && alignAfterHaltDelayTime > 0 && Time.time - lastMoveTime > alignAfterHaltDelayTime)
            {
                ForceAllLegControllerTweenMove();               
                hasAligned = true;//���Ϊ�Ѷ��룬�����ظ�����
            }
        }

    }
    void LegGroupTweenMoveNew(int index)
    {
        if (Time.time - lastMoveTime < moveLegIntervalTime)//�����ƶ�֮��Ҫ�м��������ܼ�
        {
            return;
        }
        var listTarget = listLegControllerGroup[index].listLegController;
        listTarget.ForEach(com => com.TweenMoveAsync());
        lastMoveGroupIndex = index;
        lastMoveTime = Time.time;
        hasAligned = false;
    }

    /// <summary>
    /// �����ƶ�����Leg��ָ��λ��
    /// </summary>
    public void ForceAllLegControllerTweenMove()
    {
        ListComp.ForEach(c => c.TweenMoveAsync(true));
    }

    #region Define
    [System.Serializable]
    public class LegControllerGroup
    {
        public bool NeedMove { get { return listLegController.Any(com => com.NeedMove); } }
        public float AverageDistance
        {
            get
            {
                //ToUpdate��Ӧ����ֻͳ����Ҫ�ƶ��Ľŵľ���
                _averageDistance = 0;
                listLegController.ForEach(c => _averageDistance += c.curDistance);
                _averageDistance /= listLegController.Count;
                return _averageDistance;
            }
        }//��λ��
        float _averageDistance;
        public List<CreeperLegGhostController> listLegController = new List<CreeperLegGhostController>();
    }
    #endregion


    #region Editor
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
    #endregion
}
