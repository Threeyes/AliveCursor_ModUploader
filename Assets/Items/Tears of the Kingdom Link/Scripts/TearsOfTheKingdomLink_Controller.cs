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
        //�����ƶ��ٶ��жϵ�ǰ�Ƿ����ڲ���Idle������������Ϊ0�������������ԣ�
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
    //PS����Ϊ����Ƭ�νϳ������ͨ��������Event�����ö��
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
        //ToUpdate���ͷź�ֱ�����أ������ü�������ǰ��
        goCucco.SetActive(false);
        isCuccoShowing = false;
    }

    #endregion
}
