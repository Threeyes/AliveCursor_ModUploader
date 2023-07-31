using UnityEngine;

public class OldFashion_Controller : MonoBehaviour, IAC_ModHandler
{
    #region Interface
    public void OnModInit()
    {
    }
    public void OnModDeinit()
    {
        SetSmearEffect(false);//�������ã�����Ӱ����һMod
    }
    #endregion

    public void SetSmearEffect(bool isSet)
    {
        AC_ManagerHolder.EnvironmentManager.MainCamera.clearFlags = isSet ? CameraClearFlags.Nothing : CameraClearFlags.SolidColor;
    }
}
