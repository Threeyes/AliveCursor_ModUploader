using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class TearsOfTheKingdomLink_LinkController : MonoBehaviour
{
    const string attackAnimationStateName = "Attack Blend";
    const string attackTriggerParamName = "AttackTrigger";

    public Animator animator;
    public AC_ObjectMovement_FollowTarget objectMovement;
    public MultiParentConstraint multiParentConstraint_Sword;
    public MultiParentConstraint multiParentConstraint_Shield;
    public GameObject goCucco;
    [Header("Config")]
    [Range(0, 0.5f)] public float idleThreshold = 0.2f;

    [Header("Runtime")]
    public MovingAnimationType curMovingAnimationType = MovingAnimationType.None;
    public bool isIdling = false;
    public bool isAttacking = false;
    public bool isCuccoShowing = false;
    private void Start()
    {
        HideCucco();
    }
    public void Update()
    {
        UpdateAnimationInfo();
        UpdateProps();

        //LogCurrentClipNames();
    }


    AnimatorClipInfo[] arrClipInfo;
    void UpdateAnimationInfo()
    {
         isIdling = objectMovement.CurMoveSpeedPercent <= idleThreshold;  //�����ƶ��ٶ��жϵ�ǰ�Ƿ����ڲ���Idle������������Ϊ0�������������ԣ�
        isAttacking = animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(attackAnimationStateName);//��鹥�������Ƿ񲥷���


        //#curMovingAnimationType
        if (isIdling)
        {
            curMovingAnimationType = MovingAnimationType.None;
        }
        else
        {
            //ֻȡ��һ��
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
    }

    void UpdateProps()
    {
        ///Porps��MultiParentConstraint�ۣ�
        ///-0��Hand��Idle��
        ///-1��Back��Move��
        ///-1��Toe��Move��

        //#Sword
        bool isHoldingSword = isIdling || isAttacking;
        var sourceObjects_Sword = multiParentConstraint_Sword.data.sourceObjects;
        sourceObjects_Sword.SetWeight(0, isHoldingSword ? 1f : 0f);
        sourceObjects_Sword.SetWeight(1, !isHoldingSword ? 1f : 0f);
        multiParentConstraint_Sword.data.sourceObjects = sourceObjects_Sword;

        //#Shield
        var sourceObjects_Shield = multiParentConstraint_Shield.data.sourceObjects;
        sourceObjects_Shield.SetWeight(0, isIdling ? 1f : 0f);
        sourceObjects_Shield.SetWeight(1, !isIdling && curMovingAnimationType != MovingAnimationType.Skateboarding ? 1f : 0f);
        sourceObjects_Shield.SetWeight(2, !isIdling && curMovingAnimationType == MovingAnimationType.Skateboarding ? 1f : 0f);//�ܻ�ʱ���ڽ���
        multiParentConstraint_Shield.data.sourceObjects = sourceObjects_Shield;

        //#Cucco (Show/Hide)
        if (!isCuccoShowing && curMovingAnimationType == MovingAnimationType.Hanging)
            ShowCucco();
        else if (isCuccoShowing && curMovingAnimationType != MovingAnimationType.Hanging)
            HideCucco();

    }
    void ShowCucco()
    {
        goCucco.SetActive(true);
        isCuccoShowing = true;
    }
    private void HideCucco()
    {
        //ToUpdate���ͷź�ֱ�����أ������ü�������ǰ��
        goCucco.SetActive(false);
        isCuccoShowing = false;
    }


    #region Callback
    public void OnMouseButtonDownUp(bool isDown)//Invoked by AC_CursorInputBehaviour.onButtonDownUp
    {
        if (isDown)
            animator.SetTrigger(attackTriggerParamName);
        else
            animator.ResetTrigger(attackTriggerParamName);
    }
    #endregion

    #region Debug
    public void LogCurrentClipNames()
    {
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        var arrClipInfo = animator.GetCurrentAnimatorClipInfo(0);

        //PS�������ж������Ƭ��
        string info = "";
        foreach (AnimatorClipInfo clipInfo in arrClipInfo)
        {
            if (info.NotNullOrEmpty())
                info += " + ";
            info += clipInfo.clip.name;
        }
        Debug.Log(animatorStateInfo.fullPathHash + "->" + info);

        //return arrClipInfo[0].clip.name;
    }
    #endregion

    #region Define
    public enum MovingAnimationType
    {
        None = 0,//�൱��Idle
        Run,//�ܲ�
        Hanging,//����
        Skateboarding//��ѩ
    }
    #endregion
}
