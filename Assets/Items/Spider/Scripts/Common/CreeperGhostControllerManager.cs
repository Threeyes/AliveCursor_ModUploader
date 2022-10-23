using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// 控制Ghost父物体的移动
/// 
/// Todo:
/// -完善后整合到SDK中
/// 
/// 功能：
/// -决定根模型的位置/旋转
/// </summary>
public class CreeperGhostControllerManager : ComponentGroupBase<CreeperLegGhostController>
{
    public bool IsLegsMoving { get { return ListComp.Any(c => c.isMoving); } }

    [Header("Body")]
    public Transform tfModelRoot;//模型躯干（根物体）
    public float bodyMoveSpeed = 5;
    public float bodyRotateSpeed = 0.5f;
    public float maxBodyTurn = 90;//躯干最大旋转值
    public Vector3 bodyOffsetToCenter;//躯干相对于脚中心的默认全局位移（在运行前通过调用菜单”SaveBodyCenterOffset“进行设置）
    public Transform tfGhostBody;//躯干的目标点，控制躯体的位移及旋转（单独使用一个物体控制的好处是，对躯干的修改不会影响到脚）（更改该物体的位置、旋转可实现跳跃、蹲下、转身等动作）

    [Header("Legs")]
    public float moveLegIntervalTime = 0.1f;//Warning：要比CreeperLegGhostController.tweenDuration值大，否则某个Leg会因此提前Tween完成而再次移动，从而出现某个脚频繁移动的问题
    //PS:脚需要分组（如左上对右下），每次只能移动一组脚，长途奔袭时两组脚交错移动【兼容其他爬虫的行走】
    public List<LegControllerGroup> listLegControllerGroup = new List<LegControllerGroup>();


    [Header("Runtime")]
    public int lastMoveGroupIndex = -1;
    public float lastMoveTime = 0;
    public Vector3 baseBodyPosition;
    void Start()
    {
        //ToAdd：在程序开始时或开始前记录默认的位移，因为有些躯干不在正中心【如Hand】
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
        //1.从所有脚的中心位置计算得出躯干的目标位置
        Vector3 bodyTargetPos = GetLegsCenterPos() + bodyOffsetToCenter * AC_ManagerHolder.CommonSettingManager.CursorSize;
        baseBodyPosition = Vector3.Lerp(baseBodyPosition, bodyTargetPos, Time.deltaTime * bodyMoveSpeed);//躯干的目标位置（与移动相关）

        //2.计算GhostBody的世界轴偏移量，并用其影响躯干位置（因为与音频等即时响应相关，因此不能用Lerp）
        Vector3 worldOffset = tfGhostBody.parent.TransformDirection(tfGhostBody.localPosition);//计算tfGhostBody的局部位移，并转换为全局矢量
        worldOffset *= AC_ManagerHolder.CommonSettingManager.CursorSize;//乘以光标缩放（因为目标物体同步了缩放）
        tfModelRoot.position = baseBodyPosition + worldOffset;//相对坐标不需要乘以缩放值，因为Ghost与目标物体的缩放一致，因此位置单位也一致（音频响应要求即时同步） 


        //通过tfGhostBody控制躯干的旋转
        //Todo:限制最大旋转值
        Quaternion targetRotation = tfGhostBody.rotation;
        //tfModelRoot.rotation = Quaternion.Lerp(tfModelRoot.rotation, targetRotation, Time.deltaTime * bodyRotateSpeed);
        tfModelRoot.rotation = targetRotation;//直接同步，便于及时响应音频

        //# Legs

        ///-检查哪一组需要更新位置且偏移量最大，如果是就先更新该组；同时只能有一组进行移动
        float maxGroupDistance = 0;//记录所有组中总距离最大的
        int needMoveGroupIndex = -1;
        for (int i = 0; i != listLegControllerGroup.Count; i++)
        {
            if (lastMoveGroupIndex == i)//防止同一组连续移动
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
            if (!(Time.time - lastMoveTime < moveLegIntervalTime))//两次移动之间要有间隔，否则很假
                Debug.Log(" " + needMoveGroupIndex + ": " + maxGroupDistance);
            LegGroupTweenMoveNew(needMoveGroupIndex);
        }

        //ToUpdate:在Spider静止一定时间后，强制同步GhostLegs的位置，避免强迫症患者觉得不对称
    }
    void LegGroupTweenMoveNew(int index)
    {
        if (Time.time - lastMoveTime < moveLegIntervalTime)//两次移动之间要有间隔，否则很假
        {
            return;
        }
        var listTarget = listLegControllerGroup[index].listLegController;
        listTarget.ForEach(com => com.TweenMoveAsync());
        lastMoveGroupIndex = index;
        lastMoveTime = Time.time;
    }

    /// <summary>
    /// 立刻移动所有Leg到指定位置
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
        public float TotalDistance
        {
            get
            {
                _totalDistance = 0;
                listLegController.ForEach(c => _totalDistance += c.curDistance);
                return _totalDistance;
            }
        }//总位移
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
