using UnityEngine;
using System.Collections;
/// <summary>
/// 功能：增加相对位置反向的力（以及横向的随机力，模拟摇摆的状态）
/// </summary>
public class Billboard_ClothController : ComponentHelperBase<Cloth>
    , IAC_ModHandler
    , IAC_CommonSetting_IsAliveCursorActiveHandler
    , IAC_CursorState_ChangedHandler
    , IAC_CommonSetting_CursorSizeHandler
    , IAC_SystemWindow_ChangedHandler
{
    public Vector3 relateWindForce = new Vector3(0, -1, 0);
    Transform tfParent;
    public TextAsset textAsset;

    #region Callback
    public void OnCursorSizeChanged(float value)
    {
        Resize();
    }
    public void OnIsAliveCursorActiveChanged(bool isActive)
    {
        if (isActive)
            Resize();
    }

    bool isLastHidingState;
    public void OnCursorStateChanged(AC_CursorStateInfo cursorStateInfo)
    {
        //# ZibraLiquid
        //在隐藏相关State时，临时隐藏物体
        bool isCurHidingState = IsHidingState(cursorStateInfo.cursorState);
        if (isCurHidingState)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (isLastHidingState)//只有从隐藏切换到显示，才需要更新
                Resize();
        }

        isLastHidingState = isCurHidingState;
    }

    static bool IsHidingState(AC_CursorState cursorState)//（ToUpdate：改为通用方法）
    {
        return cursorState == AC_CursorState.Exit || cursorState == AC_CursorState.Hide || cursorState == AC_CursorState.StandBy;
    }

    public void OnModInit()
    {
        Resize();
    }
    public void OnModDeinit() { }
    public void OnWindowChanged(AC_WindowEventExtArgs e)
    {
        if (e.stateChange == AC_WindowEventExtArgs.StateChange.After)
            Resize();
    }

    public void Resize()
    {
        Threeyes.Coroutine.CoroutineManager.StartCoroutineEx(IEResize());
    }

    IEnumerator IEResize()
    {
        //修复Bug: 缩放后，Y轴不同步缩放，因此需要重新激活该物体（已知问题，暂未修复 https://issuetracker.unity3d.com/issues/cloth-cloth-does-not-scale-when-in-play-mode）
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);//等待缩放不为0才能激活，否则会报错
        gameObject.SetActive(true);
    }
    #endregion

    private void Start()
    {
        tfParent = transform.parent;
    }
    void Update()
    {
        Comp.externalAcceleration = tfParent.TransformDirection(relateWindForce);//基于父物体的朝向
    }


}