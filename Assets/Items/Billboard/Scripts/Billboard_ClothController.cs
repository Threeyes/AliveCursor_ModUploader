using UnityEngine;
using System.Collections;
/// <summary>
/// 功能：增加相对位置反向的力（以及横向的随机力，模拟摇摆的状态）
/// </summary>
public class Billboard_ClothController : ComponentHelperBase<Cloth>
    , IAC_ModHandler
   , IAC_CursorState_ChangedHandler
    , IAC_CommonSetting_CursorSizeHandler
{
    public Vector3 relateWindForce = new Vector3(0, -1, 0);
    Transform tfParent;
    public TextAsset textAsset;
    #region Callback
    public void OnCursorSizeChanged(float value)
    {
        Resize();
    }
    public void OnCursorStateChanged(AC_CursorStateInfo cursorStateInfo)
    {
        //# ZibraLiquid
        //在隐藏相关State时，临时隐藏ZibraLiquid
        bool isHiding = cursorStateInfo.cursorState == AC_CursorState.Exit || cursorStateInfo.cursorState == AC_CursorState.Hide || cursorStateInfo.cursorState == AC_CursorState.StandBy;
        if (isHiding)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (!gameObject.activeInHierarchy)
                Resize();
        }
    }
    public void OnModInit()
    {
        Resize();
    }
    public void OnModDeinit() { }

    public void Resize()
    {
        //修复Bug: 缩放后，Y轴不同步缩放，因此需要重新激活该物体（已知问题，暂未修复 https://issuetracker.unity3d.com/issues/cloth-cloth-does-not-scale-when-in-play-mode）
        gameObject.SetActive(false);
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