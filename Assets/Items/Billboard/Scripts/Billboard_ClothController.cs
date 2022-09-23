using UnityEngine;
using System.Collections;
/// <summary>
/// ���ܣ��������λ�÷���������Լ�������������ģ��ҡ�ڵ�״̬��
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
        //���������Stateʱ����ʱ��������
        bool isCurHidingState = IsHidingState(cursorStateInfo.cursorState);
        if (isCurHidingState)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (isLastHidingState)//ֻ�д������л�����ʾ������Ҫ����
                Resize();
        }

        isLastHidingState = isCurHidingState;
    }

    static bool IsHidingState(AC_CursorState cursorState)//��ToUpdate����Ϊͨ�÷�����
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


}