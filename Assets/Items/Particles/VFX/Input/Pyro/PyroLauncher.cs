using UnityEngine;
using UnityEngine.VFX;
public class PyroLauncher : MonoBehaviour
{
    public Transform tfSpawnTarget;
    [SerializeField] VisualEffect _vfx = null;

    VFXEventAttribute _attrib;

    //PS:UMod有识别事件错误的问题，因此只能通过唯一方法调用
    public void OnButtonDownUp(bool isDown)
    {
        if (isDown)
            Launch();
    }
    public void Launch()
    {
        if (_attrib == null) _attrib = _vfx.CreateVFXEventAttribute();
        _attrib.SetVector3("position", tfSpawnTarget.position);

        _vfx.SendEvent("OnManualSpawn", _attrib);//发送事件
    }
}