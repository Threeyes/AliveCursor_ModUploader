using System.Collections;
using System.Collections.Generic;
using Threeyes.Coroutine;
using UnityEngine;

public class RedLantern_Controller : MonoBehaviour
    , IAC_ModHandler
    , IAC_CommonSetting_IsAliveCursorActiveHandler
    //, IAC_CursorState_ChangedHandler
    , IAC_CommonSetting_CursorSizeHandler
{
    public LineRenderer lineRenderer;
    public Transform tfRopeStart;
    public Transform tfRopeEnd;

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
        rigLantern.mass = LanternBaseMass * curScale;

        yield return null;
        gameObject.SetActive(true);
    }
}
