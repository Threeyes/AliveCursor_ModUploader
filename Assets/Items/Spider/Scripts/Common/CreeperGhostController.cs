using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Threeyes.Coroutine;
/// <summary>
/// 
/// Todo:
/// -完善后整合到SDK中
/// 
/// 功能：
/// -决定根模型的位置/旋转
/// </summary>
public class CreeperGhostController : MonoBehaviour
{
    public bool IsLegsMoving { get { return listLegController1.Any(c => c.isMoving) || listLegController2.Any(c => c.isMoving); } }

    [Header("Body")]
    public Transform tfModelRoot;//模型躯干（根物体）
    public float bodyMoveSpeed = 5;
    public float bodyRotateSpeed = 0.5f;
    public float maxBodyTurn = 90;//躯干最大旋转值
    public Transform tfGhostBody;//躯干的目标点，控制躯体的位移及旋转（单独使用一个物体控制的好处是，对躯干的修改不会影响到脚）（更改该物体的位置、旋转可实现跳跃、蹲下、转身等动作）

    [Header("Legs")]
    //ToUpdate：封装为数据类，方便后续拓展
    //PS:脚需要分组（如左上对右下），每次只能移动一组脚，长途奔袭时两组脚交错移动【兼容其他爬虫的行走】
    public List<CreeperLegGhostController> listLegController1 = new List<CreeperLegGhostController>();
    public List<CreeperLegGhostController> listLegController2 = new List<CreeperLegGhostController>();
    public float moveLegIntervalTime = 0.2f;
    [Header("Runtime")]
    public int lastMoveGroupIndex = -1;
    public float lastMoveTime = 0;

    public Vector3 baseBodyPosition;

    private void Start()
    {
        baseBodyPosition = tfModelRoot.position;
    }
    private void LateUpdate()
    {
        //# Body
        //让模型根物体(躯干)跟该Ghost同步

        //躯干应该是动态根据脚的位置计算的
        Vector3 centerPos = Vector3.zero;
        listLegController1.ForEach(com => centerPos += com.tfSourceTarget.position);
        listLegController2.ForEach(com => centerPos += com.tfSourceTarget.position);
        centerPos /= (listLegController1.Count + listLegController2.Count);
        baseBodyPosition = Vector3.Lerp(baseBodyPosition, centerPos, Time.deltaTime * bodyMoveSpeed);// tfGhostBody.position;

        //#计算GhostBody的世界轴偏移量
        Vector3 worldOffset = tfGhostBody.parent.TransformDirection(tfGhostBody.localPosition);//转换为矢量
        worldOffset *= AC_ManagerHolder.CommonSettingManager.CursorSize;//同步缩放（因为目标物体为缩放）
        tfModelRoot.position = baseBodyPosition + worldOffset;//相对坐标不需要乘以缩放值，因为Ghost与目标物体的缩放一致，因此位置单位也一致（音频响应要求即时同步） 


        //通过tfGhostBody控制躯干的旋转
        //Todo:限制最大旋转值
        Quaternion targetRotation = tfGhostBody.rotation;
        //tfModelRoot.rotation = Quaternion.Lerp(tfModelRoot.rotation, targetRotation, Time.deltaTime * bodyRotateSpeed);
        tfModelRoot.rotation = targetRotation;

        //# Legs
        ///-检查哪一组需要更新位置且偏移量最大，如果是就先更新该组；同时只能有一组进行移动
        bool shouldGroup1Move = listLegController1.Any(com => com.NeedMove);
        bool shouldGroup2Move = listLegController2.Any(com => com.NeedMove);
        if (shouldGroup1Move && shouldGroup2Move)
        {
            ///-记录上次移动的组，如果已经移动了且本次2组都需要移动，则上一组就无法连续移动，改为另一组先移动
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

        //ToUpdate:在Spider静止一定时间后，强制同步GhostLegs的位置，避免强迫症患者觉得不对称
    }

    public void LegGroupForceTweenMove()
    {
        //LegGroupTweenMove(0, true);
        //LegGroupTweenMove(1, true);
        listLegController1.ForEach(c => c.TweenMoveAsync(true));
        listLegController2.ForEach(c => c.TweenMoveAsync(true));
    }
    void LegGroupTweenMove(int index)
    {
        if (Time.time - lastMoveTime < moveLegIntervalTime)//两次移动之间要有间隔，否则很假
        {
            return;
        }
        var listTarget = (index == 1 ? listLegController1 : listLegController2);
        listTarget.ForEach(com => com.TweenMoveAsync());
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
