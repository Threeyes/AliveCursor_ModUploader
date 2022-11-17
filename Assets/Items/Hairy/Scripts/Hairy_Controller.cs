using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.DemoTeam.Hair;/ToFix：/无法引用的原因可能是其Assembled文件丢失了部分引用

/// <summary>
///
/// Todo:
/// -Working重力与朝向一致（参考Billboard）
/// </summary>
public class Hairy_Controller : MonoBehaviour
    , IAC_CursorState_ChangedHandler
{
    public Vector3 relateWindForce = new Vector3(0, -1, 0);
    public AC_HairInstanceController hairInstanceController;

    #region Callback
    //Invoke by CIB
    public void SetGlobalPositionInterval(float value)
    {
        hairInstanceController.Config.globalPositionInfluence = value;
        hairInstanceController.UpdateSetting();
    }

    bool isBoredState = false;
    public void OnCursorStateChanged(AC_CursorStateInfo cursorStateInfo)
    {
        //PS：Wander时重力变小
        if (cursorStateInfo.cursorState == AC_CursorState.Bored)
        {
            if (cursorStateInfo.stateChange == AC_CursorStateInfo.StateChange.Enter)
            {
                hairInstanceController.Config.gravity = 0;
                isBoredState = true;
            }
            else
            {
                hairInstanceController.Config.gravity = 1;
                isBoredState = false;
            }

            hairInstanceController.UpdateSetting();
        }

    }
    #endregion

    Transform tfParent;
    private void Start()
    {
        tfParent = hairInstanceController.transform.parent;
    }
    public float updateConfigFrequence = 1;
    float lastUpdateConfigTime = 0;
    void Update()
    {
        if (isBoredState)
            return;

        if (Time.time - lastUpdateConfigTime < updateConfigFrequence)
            return;

        //非Bored时根据方向设置重力，主要用于自由旋转的状态

        //PS：为了避免性能问题，要减少更新频率，且只有在Working状态下有效
        hairInstanceController.Config.gravityRotation =
            tfParent.eulerAngles;
        hairInstanceController.UpdateSetting();
        lastUpdateConfigTime = Time.time;
    }

}
