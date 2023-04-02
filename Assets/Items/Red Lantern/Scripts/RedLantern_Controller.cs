using System.Collections;
using System.Collections.Generic;
using Threeyes.Coroutine;
using UnityEngine;

public class RedLantern_Controller : MonoBehaviour
    , IAC_ModHandler
    , IAC_CommonSetting_IsAliveCursorActiveHandler
    , IAC_CursorState_ChangedHandler
    , IAC_CommonSetting_CursorSizeHandler
{
    public Transform tfRopeStartTarget;
    public LineRenderer lineRenderer;
    public Transform tfRopeStart;
    public Transform tfRopeEnd;

    public Transform tfPartGroup;//存储所有零部件
    public Rigidbody rigRopeStart;
    public Rigidbody rigLantern;
    public float ropeBaseWidth = 0.010f;
    public float LanternBaseMass = 2.5f;//(PS:Weight will affect Rope's length)

    private void Update()
    {
        lineRenderer.SetPosition(0, tfRopeStart.position);
        lineRenderer.SetPosition(1, tfRopeEnd.position);

        if (AC_AliveCursor.Instance)
            lineRenderer.startWidth = lineRenderer.endWidth = ropeBaseWidth * AC_AliveCursor.Instance.transform.localScale.x;//Sync scale（Place here in case current is Hide state)
    }

    void FixedUpdate()
    {
        rigRopeStart.MovePosition(tfRopeStartTarget.position);
        rigRopeStart.MoveRotation(tfRopeStartTarget.rotation);
    }

    #region Callback
    public void OnModInit()
    {
        Resize();
    }
    public void OnModDeinit() { }
    public void OnIsAliveCursorActiveChanged(bool isActive)
    {
        if (isActive)
            Resize();
    }
    public void OnCursorSizeChanged(float value)
    {
        Resize();
    }
    public void OnCursorStateChanged(AC_CursorStateInfo cursorStateInfo)
    {
        bool isVanishState = AC_ManagerHolder.StateManager.IsVanishState(cursorStateInfo.cursorState);
        tfPartGroup.gameObject.SetActive(!isVanishState);
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
        //让Joint相关组件强制更新
        gameObject.SetActive(false);

        float curScale = AC_ManagerHolder.CommonSettingManager.CursorSize;
        tfPartGroup.localScale = Vector3.one * curScale;
        rigLantern.mass = LanternBaseMass * curScale;
        yield return null;
        gameObject.SetActive(true);
        rigLantern.WakeUp();
    }
}
