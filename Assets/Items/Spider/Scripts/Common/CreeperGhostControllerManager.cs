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
public class CreeperGhostControllerManager : MonoBehaviour
{
    public bool IsLegsMoving { get { return listAllLegController.Any(c => c.isMoving); } }

    [Header("Body")]
    public Transform tfModelRoot;//ģ�����ɣ������壩
    public float bodyMoveSpeed = 5;
    public float bodyRotateSpeed = 0.5f;
    public float maxBodyTurn = 90;//���������תֵ
    public Transform tfGhostBody;//���ɵ�Ŀ��㣬���������λ�Ƽ���ת������ʹ��һ��������Ƶĺô��ǣ������ɵ��޸Ĳ���Ӱ�쵽�ţ������ĸ������λ�á���ת��ʵ����Ծ�����¡�ת��ȶ�����

    [Header("Legs")]
    //PS:����Ҫ���飨�����϶����£���ÿ��ֻ���ƶ�һ��ţ���;��Ϯʱ����Ž����ƶ�������������������ߡ�
    public List<LegControllerGroup> listLegControllerGroup = new List<LegControllerGroup>();
    public float moveLegIntervalTime = 0.1f;//Warning��Ҫ��CreeperLegGhostController.tweenDurationֵ�󣬷���ĳ��Leg��Ƶ���ƶ�


    [Header("Runtime")]
    public int lastMoveGroupIndex = -1;
    public float lastMoveTime = 0;
    public Vector3 baseBodyPosition;
    List<CreeperLegGhostController> listAllLegController = new List<CreeperLegGhostController>();
    private void Start()
    {
        //ToAdd���ڳ���ʼʱ��ʼǰ��¼Ĭ�ϵ�λ�ƣ���Ϊ��Щ���ɲ��������ġ���Hand��
        baseBodyPosition = tfModelRoot.position;

        //��������Leg
        listAllLegController.Clear();
        foreach (var lcg in listLegControllerGroup)
        {
            foreach (var lc in lcg.listLegController)
                listAllLegController.Add(lc);
        }
    }
    private void LateUpdate()
    {
        //# Body
        //��ģ�͸�����(����)����Ghostͬ��

        //�������ɵ�����λ�ã��ӽŵ�����λ�ü���ó�
        Vector3 centerPos = Vector3.zero;
        listAllLegController.ForEach(com => centerPos += com.tfSourceTarget.position);
        centerPos /= listAllLegController.Count;
        baseBodyPosition = Vector3.Lerp(baseBodyPosition, centerPos, Time.deltaTime * bodyMoveSpeed);// tfGhostBody.position;

        //#����GhostBody��������ƫ����
        Vector3 worldOffset = tfGhostBody.parent.TransformDirection(tfGhostBody.localPosition);//ת��Ϊʸ��
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
            if (lcg.NeedMove && lcg.TotalDistance > maxGroupDistance)
            {
                needMoveGroupIndex = i;
                maxGroupDistance = lcg.TotalDistance;
            }
        }
        if (needMoveGroupIndex >= 0)
        {
            if (!(Time.time - lastMoveTime < moveLegIntervalTime))//�����ƶ�֮��Ҫ�м��������ܼ�
                Debug.Log(" " + needMoveGroupIndex + ": " + maxGroupDistance);
            LegGroupTweenMoveNew(needMoveGroupIndex);
        }

        //ToUpdate:��Spider��ֹһ��ʱ���ǿ��ͬ��GhostLegs��λ�ã�����ǿ��֢���߾��ò��Գ�
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
    }

    /// <summary>
    /// �����ƶ�����Leg��ָ��λ��
    /// </summary>
    public void ForceAllLegControllerTweenMove()
    {
        listAllLegController.ForEach(c => c.TweenMoveAsync(true));
    }

    #region Define
    [System.Serializable]
    public class LegControllerGroup
    {
        public bool NeedMove { get { return listLegController.Any(com => com.NeedMove); } }
        public float TotalDistance
        {
            get
            {
                _totalDistance = 0;
                listLegController.ForEach(c => _totalDistance += c.curDistance);
                return _totalDistance;
            }
        }//��λ��
        float _totalDistance;
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
