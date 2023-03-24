using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.DemoTeam.Hair;
using static Unity.DemoTeam.Hair.HairInstance;
using Threeyes.Coroutine;
using static Unity.DemoTeam.Hair.HairAsset;
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

    public List<HairAsset> listHairAsset = new List<HairAsset>();

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

    public HairAsset hairAsset;
    //Invoked by PD
    public void OnHairLengthChanged(int index)
    {
        //if (index + 1 >= listHairAsset.Count)
        //    return;
        GroupInstance[] arrInstance = hairInstanceController.Comp.strandGroupInstances;
        GroupInstance groupInstanceFirst = arrInstance[0];
        groupInstanceFirst.groupAssetReference = new GroupAssetReference() { hairAsset = listHairAsset[index] };
        arrInstance[0] = groupInstanceFirst;
        hairInstanceController.Comp.strandGroupInstances = arrInstance;

        ////运行时更新【Bug：0.9版本会导致闪退，0.10可用。暂时无法使用，因为只能通过编辑器代码BuildHairAsset。解决后就可以直接暴露其所有参数，替换掉上面的死板实现】
        //SettingsProcedural settingsProcedural = hairAsset.settingsProcedural;
        //settingsProcedural.strandLength = index * 0.5f;
        //hairAsset.settingsProcedural = settingsProcedural;
        //HairAssetBuilder.BuildHairAsset(hairAsset);//【Warning：仅编辑器可用】

        ReBuild();//重新刷新
    }
    void ReBuild()
    {
        CoroutineManager.StartCoroutineEx(IEReBuild());
    }
    private IEnumerator IEReBuild()
    {
        hairInstanceController.Comp.enabled = false;
        yield return null;
        hairInstanceController.Comp.enabled = true;
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
