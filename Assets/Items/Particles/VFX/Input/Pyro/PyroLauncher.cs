using UnityEngine;
using UnityEngine.VFX;
public class PyroLauncher : MonoBehaviour
{
    [SerializeField] VisualEffect _vfx = null;

    VFXEventAttribute _attrib;

    public void Launch()
    {
        Camera _camera = AC_ManagerHolder.EnvironmentManager.MainCamera;
        var pos = AC_ManagerHolder.SystemCursorManager.MousePosition;
        pos.z = _vfx.transform.position.z - _camera.transform.position.z;
        pos = _camera.ScreenToWorldPoint(pos);

        if (_attrib == null) _attrib = _vfx.CreateVFXEventAttribute();
        _attrib.SetVector3("position", pos);

        _vfx.SendEvent("OnManualSpawn", _attrib);//·¢ËÍÊÂ¼þ
    }
}