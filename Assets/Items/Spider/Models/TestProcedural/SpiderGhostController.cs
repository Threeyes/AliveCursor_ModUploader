using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// 
/// Todo:
/// -取名为更通用，如爬虫，Ghost改为Guider
/// -完善后整合到SDK中
/// 
/// 功能：
/// -决定根模型的位置/旋转
/// </summary>
public class SpiderGhostController : MonoBehaviour
{
    public Transform tfModelRoot;//模型根物体(躯干)

    [Header("Body")]
    public float bodyMoveSpeed = 5;
    public Vector3 bodyOffset = Vector3.zero;

    public Transform tfGhostBody;//单独控制躯体的旋转，不影响脚的旋转及位置。（需要限制其活动范围为脚内部）

    [Header("Leg")]
    //ToUpdate：封装为数据类，方便后续拓展
    //PS:脚需要分组（如左上对右下），每次只能更新一组脚【兼容其他脚的行走】
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
        //让模型根物体(躯干)跟该Ghost同步

        //躯干应该是动态根据脚的位置计算的
        Vector3 centerPos = Vector3.zero;
        listLegController1.ForEach(com => centerPos += com.tfSourceTarget.position);
        listLegController2.ForEach(com => centerPos += com.tfSourceTarget.position);
        centerPos /= (listLegController1.Count + listLegController2.Count);
        tfModelRoot.position = Vector3.Lerp(tfModelRoot.position, centerPos, Time.deltaTime * bodyMoveSpeed) + bodyOffset;// tfGhostBody.position;

        //通过tfGhostBody控制躯干的旋转
        tfModelRoot.rotation = tfGhostBody.rotation;

        ///Todo:
        ///-检查哪一组需要更新位置且偏移量最大，如果是就先更新该组；同时只能有一组进行移动
        ///-将移动方法作为Async，方便该类调用
        bool shouldGroup1Move = listLegController1.Any(com => com.NeedMove);
        bool shouldGroup2Move = listLegController2.Any(com => com.NeedMove);

        if (shouldGroup1Move && shouldGroup2Move)
        {
            ///-记录上次移动的组，如果已经移动了且本次2组都需要移动，则上一组就无法连续移动，改为另一组先移动
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
        if (Time.time - lastMoveTime < moveLegIntervalTime)//两次移动之间要有间隔，否则很假
        {
            return;
        }
        var listTarget = (index == 1 ? listLegController1 : listLegController2);
        listTarget.ForEach(com => com.TweenMoveAsync());
        lastMoveGroupIndex = index;
        lastMoveTime = Time.time;
    }
}
