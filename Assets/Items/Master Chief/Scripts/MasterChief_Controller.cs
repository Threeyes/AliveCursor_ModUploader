using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// Bored:Middle Click
/// </summary>
public class MasterChief_Controller : MonoBehaviour
    , IAC_CursorState_ChangedHandler
{
    //ͨ��cursorInputBehaviourCollection_Attackͳһ��������AC_CursorInputBehaviour�ĵ��ã�����ModifyKeys��ͳһ����
    public AC_CursorInputBehaviourCollection cursorInputBehaviourCollection_Attack;
    public AC_CursorInputBehaviour cursorInputBehaviour_Left;
    public AC_CursorInputBehaviour cursorInputBehaviour_Right;

    [Header("Scene Setup")]
    public Transform tfGrenadeThrowPoint;
    public GameObject goRootCanvas;

    [Header("Prefabs")]
    public GameObject preHumanPistolMuzzle;
    public GameObject preHumanGrenade;


    [Header("Config")]
    public AttackType attackType = AttackType.HumanWeapon;
    public MasterChief_Grenade.ConfigInfo grenadeConfig;
    private void Awake()
    {
        cursorInputBehaviourCollection_Attack.onLeftButtonDownUp.AddListener(OnLeftButtonDownUp);
        cursorInputBehaviourCollection_Attack.onRightButtonDownUp.AddListener(OnRightButtonDownUp);
        cursorInputBehaviourCollection_Attack.onMiddleButtonDownUp.AddListener(OnMiddleButtonDownUp);
    }

    //#Gun
    private void OnLeftButtonDownUp(bool isDown)
    {
        if (isDown)
            cursorInputBehaviour_Left.Play();
        else
            cursorInputBehaviour_Left.Stop();

        if (isDown)
        {
            GameObject preTarget = null;
            switch (attackType)
            {
                case AttackType.HumanWeapon:
                    preTarget = preHumanPistolMuzzle; break;
                default:
                    Debug.LogError("Not Define!"); break;
            }

            if (preTarget)
            {
                Vector3 curPos = AC_ManagerHolder.SystemCursorManager.WorldPosition;
                Quaternion quaternion = Quaternion.LookRotation(curPos - AC_ManagerHolder.EnvironmentManager.MainCamera.transform.position, Vector3.up);/*   default(Quaternion)*/;//ToUpdate,Ӧ�����������Ŀ������תֵ

                preTarget.InstantiatePrefab(null, curPos, quaternion);
            }
        }
    }

    //#Grenade
    public float grenadeThrowPower = 1;
    //Ref:makegeo-TerrainDamager����+  https://github.com/kurtdekker/makegeo/blob/master/makegeo/Assets/Scripts/WeaponGrenadeTosser.cs
    private void OnMiddleButtonDownUp(bool isDown)
    {
        if (isDown)
        {
            GameObject preTarget = null;
            switch (attackType)
            {
                case AttackType.HumanWeapon:
                    preTarget = preHumanGrenade; break;
                default:
                    Debug.LogError("Not Define!"); break;
            }

            if (preTarget)
            {
                MasterChief_Grenade masterChief_Grenade = preTarget.InstantiatePrefab<MasterChief_Grenade>(null, tfGrenadeThrowPoint.position);
                masterChief_Grenade.transform.localScale = Vector3.one * AC_ManagerHolder.CommonSettingManager.CursorSize;
                masterChief_Grenade.Init(grenadeConfig);

                //��Ŀ�ĵ���
                Vector3 targetPos = AC_ManagerHolder.SystemCursorManager.WorldPosition;
                Vector3 tossVelocity = (targetPos - tfGrenadeThrowPoint.position).normalized;
                masterChief_Grenade.AddVelocity(tossVelocity * grenadeThrowPower);
            }
        }
    }

    //#Knife/Sword
    private void OnRightButtonDownUp(bool isDown)
    {
        if (isDown)
            cursorInputBehaviour_Right.Play();
        else
            cursorInputBehaviour_Right.Stop();

    }

    #region Callback
    public void OnCursorStateChanged(AC_CursorStateInfo cursorStateInfo)
    {
        ///Bored��
        ///-������׷��
        ///-��ʱ����UI�����ӳ���
        if (cursorStateInfo.cursorState == AC_CursorState.Bored)
        {
            if (cursorStateInfo.stateChange == AC_CursorStateInfo.StateChange.Enter)
            {
                goRootCanvas.SetActive(false);
                OnRightButtonDownUp(true);
            }
            else
            {
                goRootCanvas.SetActive(true);
                OnRightButtonDownUp(false);
            }
        }
    }
    #endregion

    public enum AttackType
    {
        HumanWeapon,
        CovenantWeapon,
        BareHanded,
    }
}
