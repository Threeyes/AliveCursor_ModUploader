using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class TearsOfTheKingdomLink_LinkController : MonoBehaviour
{
    public Animator animator;
    public AC_ObjectMovement_FollowTarget objectMovement;
    public MultiParentConstraint multiParentConstraint_Sword;
    public MultiParentConstraint multiParentConstraint_Shield;
    public GameObject goCucco;
    [Header("Config")]
    [Range(0, 0.5f)] public float idleThreshold = 0.2f;

    [Header("Runtime")]
    public MovingAnimationType curMovingAnimationType = MovingAnimationType.None;
    public bool isLastIdle = false;
    public bool isCuccoShowing = false;
    private void Start()
    {
        HideCucco();
    }
    public void Update()
    {
        UpdateAnimationInfo();

        UpdateProps();


        //LogCurrentClipName();
    }


    AnimatorClipInfo[] arrClipInfo;
    void UpdateAnimationInfo()
    {
        if (objectMovement.CurMoveSpeedPercent <= idleThreshold)
        {
            curMovingAnimationType = MovingAnimationType.None;
            return;
        }

        //只取第一个
        arrClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (arrClipInfo.Length > 0)
        {
            string clipName = arrClipInfo[0].clip.name;
            if (clipName.Contains(MovingAnimationType.Run.ToString()))
            {
                curMovingAnimationType = MovingAnimationType.Run;
            }
            else if (clipName.Contains(MovingAnimationType.Hanging.ToString()))
            {
                curMovingAnimationType = MovingAnimationType.Hanging;
            }
            else if (clipName.Contains(MovingAnimationType.Skateboarding.ToString()))
            {
                curMovingAnimationType = MovingAnimationType.Skateboarding;
            }
        }
    }

    void UpdateProps()
    {
        //根据移动速度判断当前是否正在播放Idle动作（不设置为0可以增加流畅性）
        bool isCurIdle = objectMovement.CurMoveSpeedPercent <= idleThreshold;

        ///Porps的MultiParentConstraint槽：
        ///-0：Hand（Idle）
        ///-1：Back（Move）
        ///-1：Toe（Move）

        //#Sword
        var sourceObjects_Sword = multiParentConstraint_Sword.data.sourceObjects;
        sourceObjects_Sword.SetWeight(0, isCurIdle ? 1f : 0f);
        sourceObjects_Sword.SetWeight(1, !isCurIdle ? 1f : 0f);
        multiParentConstraint_Sword.data.sourceObjects = sourceObjects_Sword;

        //#Shield
        var sourceObjects_Shield = multiParentConstraint_Shield.data.sourceObjects;
        sourceObjects_Shield.SetWeight(0, isCurIdle ? 1f : 0f);
        sourceObjects_Shield.SetWeight(1, !isCurIdle && curMovingAnimationType != MovingAnimationType.Skateboarding ? 1f : 0f);
        sourceObjects_Shield.SetWeight(2, !isCurIdle && curMovingAnimationType == MovingAnimationType.Skateboarding ? 1f : 0f);//盾滑时盾在脚下
        multiParentConstraint_Shield.data.sourceObjects = sourceObjects_Shield;

        //#Cucco (Show/Hide)
        if (!isCuccoShowing && curMovingAnimationType == MovingAnimationType.Hanging)
            ShowCucco();
        else if (isCuccoShowing && curMovingAnimationType != MovingAnimationType.Hanging)
            HideCucco();

        if (isCurIdle != isLastIdle)//Update on state changed
        {
            ///PS：上面的方法不能放到该块中，因为可能状态切换瞬间，枚举还未更新
            isLastIdle = isCurIdle;
        }
    }
    void ShowCucco()
    {
        goCucco.SetActive(true);
        isCuccoShowing = true;
    }
    private void HideCucco()
    {
        //ToUpdate：释放后不直接隐藏，而是让鸡继续往前飞
        goCucco.SetActive(false);
        isCuccoShowing = false;
    }

    #region Debug
    public void LogCurrentClipName()
    {
        var arrClipInfo = animator.GetCurrentAnimatorClipInfo(0);

        //PS：可能有多个动画片段
        string info = "";
        foreach (var clipInfo in arrClipInfo)
        {
            if (info.NotNullOrEmpty())
                info += " + ";
            info += clipInfo.clip.name;
        }
        Debug.Log(info);

        //return arrClipInfo[0].clip.name;
    }
    #endregion

    #region Define
    public enum MovingAnimationType
    {
        None = 0,//相当于Idle
        Run,//跑步
        Hanging,//滑翔
        Skateboarding//滑雪
    }
    #endregion
}
