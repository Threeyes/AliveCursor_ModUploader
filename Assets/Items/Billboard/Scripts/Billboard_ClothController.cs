using UnityEngine;
using System.Collections;
using Threeyes.GameFramework;
using Threeyes.Core;
/// <summary>
/// ���ܣ��������λ�÷���������Լ�������������ģ��ҡ�ڵ�״̬��
/// </summary>
public class Billboard_ClothController : ComponentHelperBase<Cloth>
    , IAC_ModHandler
    , IAC_CommonSetting_IsAliveCursorActiveHandler
    , IAC_CursorState_ChangedHandler
    , IAC_CommonSetting_CursorSizeHandler
    , IHubSystemWindow_ChangeCompletedHandler
{
    public Vector3 relateWindForce = new Vector3(0, -1, 0);
    Transform tfParent;

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

    public void OnModInit()
    {
        Resize();
    }
    public void OnModDeinit() { }
    public void OnWindowChangeCompleted()
    {
            Resize();
    }

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
        //�޸�Bug: ���ź�Y�᲻ͬ�����ţ������Ҫ���¼�������壨��֪���⣬��δ�޸� https://issuetracker.unity3d.com/issues/cloth-cloth-does-not-scale-when-in-play-mode��
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);//�ȴ����Ų�Ϊ0���ܼ������ᱨ��
        gameObject.SetActive(true);
    }
    #endregion

    private void Start()
    {
        tfParent = transform.parent;
    }
    void Update()
    {
        Comp.externalAcceleration = tfParent.TransformDirection(relateWindForce);//���ڸ�����ĳ���
    }


    static bool IsHidingState(AC_CursorState cursorState)//��ToUpdate����Ϊͨ�÷�����
    {
        return cursorState == AC_CursorState.Exit || cursorState == AC_CursorState.Hide || cursorState == AC_CursorState.StandBy;
    }
}