using System.Collections;
using System.Collections.Generic;
using Threeyes.Coroutine;
using UnityEngine;
/// <summary>
/// Func:
/// -Sync Cursor Scale
/// </summary>
public class CreeperController : MonoBehaviour
    , IAC_ModHandler
    , IAC_CommonSetting_IsAliveCursorActiveHandler
    , IAC_CursorState_ChangedHandler
    , IAC_CommonSetting_CursorSizeHandler
{
    public CreeperGhostControllerManager creeperGhostController;
    public Transform tfLegTargetGroup;
  
    #region Callback
    public void OnModInit()
    {
        Resize();
    }
    public void OnModDeinit()
    {
    }

    public void OnIsAliveCursorActiveChanged(bool isActive)
    {
        if (isActive)
            Resize();
        else
            gameObject.SetActive(false);
    }

    bool isLastHidingState;
    public void OnCursorStateChanged(AC_CursorStateInfo cursorStateInfo)
    {
        //���������Stateʱ����ʱ���ظ�����
        bool isCurHidingState = IsHidingState(cursorStateInfo.cursorState);
        if (isCurHidingState)
        {
            TryStopResizeCoroutine();
            gameObject.SetActive(false);
        }
        else
        {
            if (isLastHidingState)//ֻ�д������л�����ʾ������Ҫ����
                Resize();
        }
        isLastHidingState = isCurHidingState;
    }

    public void OnCursorSizeChanged(float value)
    {
        Resize();
    }
    #endregion

    protected Coroutine cacheEnumResize;
    public void Resize()
    {
        TryStopResizeCoroutine();
        cacheEnumResize = CoroutineManager.StartCoroutineEx(IEResize());
    }
    protected virtual void TryStopResizeCoroutine()
    {
        if (cacheEnumResize != null)
            CoroutineManager.StopCoroutineEx(cacheEnumResize);
    }
    IEnumerator IEResize()
    {
        //��Rig������ǿ�Ƹ���(���ź���Ҫ��������������RigBuilder�������)
        gameObject.SetActive(false);
        tfLegTargetGroup.localScale = gameObject.transform.localScale = Vector3.one * AC_ManagerHolder.CommonSettingManager.CursorSize;
        //���¹ؽ�
        creeperGhostController.ForceAllLegControllerTweenMove();
        //yield return new WaitForSeconds(0.1f);//�ȴ����Ų�Ϊ0���ܼ������ᱨ��
        yield return null;
        gameObject.SetActive(true);
    }

    static bool IsHidingState(AC_CursorState cursorState)//��ToUpdate����Ϊͨ�÷�����
    {
        return cursorState == AC_CursorState.Exit || cursorState == AC_CursorState.Hide || cursorState == AC_CursorState.StandBy;
    }

}
