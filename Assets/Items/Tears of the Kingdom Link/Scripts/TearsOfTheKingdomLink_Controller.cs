using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class TearsOfTheKingdomLink_Controller : MonoBehaviour
{
    public AC_ObjectMovement_FollowTarget objectMovement;
    public MultiParentConstraint multiParentConstraint_Sword;
    public MultiParentConstraint multiParentConstraint_Shield;
    public GameObject goCucco;
    //Config
    [Range(0, 0.5f)] public float idleThreshold = 0;

    private void Start()
    {
        HideCucco();
    }
    public void Update()
    {
        UpdateProps();

        if (isCuccoShowing && objectMovement.CurMoveSpeedPercent < 1)
            HideCucco();
    }

    bool isLastIdle = false;
    void UpdateProps()
    {
        //根据移动速度判断当前是否正在播放Idle动作（不设置为0可以增加流畅性）
        bool isCurIdle = objectMovement.CurMoveSpeedPercent <= idleThreshold;

        if (isCurIdle != isLastIdle)
        {
            var sourceObjects_Sword = multiParentConstraint_Sword.data.sourceObjects;
            sourceObjects_Sword.SetWeight(0, !isCurIdle ? 1f : 0f);
            sourceObjects_Sword.SetWeight(1, isCurIdle ? 1f : 0f);
            multiParentConstraint_Sword.data.sourceObjects = sourceObjects_Sword;

            var sourceObjects_Shield = multiParentConstraint_Shield.data.sourceObjects;
            sourceObjects_Shield.SetWeight(0, !isCurIdle ? 1f : 0f);
            sourceObjects_Shield.SetWeight(1, isCurIdle ? 1f : 0f);
            multiParentConstraint_Shield.data.sourceObjects = sourceObjects_Sword;

        }
        isLastIdle = isCurIdle;
    }

    #region Invoked by  AnimationEvent
    bool isCuccoShowing = true;
    //PS：因为动画片段较长，因此通过多增加Event来调用多次
    public void OnAniamtionHanging_ShowCucco()
    {
        if (!isCuccoShowing)
        {
            goCucco.SetActive(true);
            isCuccoShowing = true;
        }
    }
    private void HideCucco()
    {
        //ToUpdate：释放后不直接隐藏，而是让鸡继续往前飞
        goCucco.SetActive(false);
        isCuccoShowing = false;
    }

    #endregion
}
