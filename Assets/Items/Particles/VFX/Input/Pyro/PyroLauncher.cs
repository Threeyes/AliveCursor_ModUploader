using UnityEngine;
using UnityEngine.VFX;
public class PyroLauncher : MonoBehaviour
{
    public Transform tfSpawnTarget;
    [SerializeField] VisualEffect _vfx = null;

    VFXEventAttribute _attrib;

    //PS:UMod��ʶ���¼���������⣬���ֻ��ͨ��Ψһ��������
    public void OnButtonDownUp(bool isDown)
    {
        if (isDown)
            Launch();
    }
    public void Launch()
    {
        if (_attrib == null) _attrib = _vfx.CreateVFXEventAttribute();
        _attrib.SetVector3("position", tfSpawnTarget.position);

        _vfx.SendEvent("OnManualSpawn", _attrib);//�����¼�
    }
}