using UnityEngine;
using System.Collections;
/// <summary>
/// ���ܣ��������λ�÷���������Լ�������������ģ��ҡ�ڵ�״̬��
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
        //���������Stateʱ����ʱ����ZibraLiquid
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
        //�޸�Bug: ���ź�Y�᲻ͬ�����ţ������Ҫ���¼�������壨��֪���⣬��δ�޸� https://issuetracker.unity3d.com/issues/cloth-cloth-does-not-scale-when-in-play-mode��
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
        Comp.externalAcceleration = tfParent.TransformDirection(relateWindForce);//���ڸ�����ĳ���
    }
}