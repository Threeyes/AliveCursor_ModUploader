using UnityEngine;

public class OldFashion_Controller : MonoBehaviour, IAC_ModHandler
{
    #region Interface
    public void OnModInit()
    {
    }
    public void OnModDeinit()
    {
        SetSmearEffect(false);//重置设置，以免影响下一Mod
    }
    #endregion

    public void SetSmearEffect(bool isSet)
    {
        AC_ManagerHolder.EnvironmentManager.MainCamera.clearFlags = isSet ? CameraClearFlags.Nothing : CameraClearFlags.SolidColor;
    }
}
