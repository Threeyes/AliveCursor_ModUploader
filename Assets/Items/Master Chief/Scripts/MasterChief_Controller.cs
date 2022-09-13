using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterChief_Controller : MonoBehaviour
{
    public AC_CursorInputBehaviourCollection cursorInputBehaviourCollection_Attack;

    [Header("Scene Setup")]
    public Transform tfGrenadeThrowPoint;

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

    private void OnLeftButtonDownUp(bool isDown)
    {
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
                Quaternion quaternion = Quaternion.LookRotation(curPos - AC_ManagerHolder.EnvironmentManager.MainCamera.transform.position, Vector3.up);/*   default(Quaternion)*/;//ToUpdate,应该是相机朝向目标点的旋转值

                preTarget.InstantiatePrefab(null, curPos, quaternion);
            }
        }
    }

    //Grenade
    public float grenadeThrowPower = 1;
    //Ref:makegeo-TerrainDamager场景+  https://github.com/kurtdekker/makegeo/blob/master/makegeo/Assets/Scripts/WeaponGrenadeTosser.cs
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

                //朝目的地扔
                Vector3 targetPos = AC_ManagerHolder.SystemCursorManager.WorldPosition;
                Vector3 tossVelocity = (targetPos - tfGrenadeThrowPoint.position).normalized;
                masterChief_Grenade.AddVelocity(tossVelocity * grenadeThrowPower);
            }
        }
    }
    private void OnRightButtonDownUp(bool isDown)
    {

    }

    public enum AttackType
    {
        HumanWeapon,
        CovenantWeapon,
        BareHanded,
    }
}
