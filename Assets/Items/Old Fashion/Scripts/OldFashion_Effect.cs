using UnityEngine;

public class OldFashion_Effect : MonoBehaviour
{
    public void SetSmearEffect(bool isSet)
    {
        AC_ManagerHolder.EnvironmentManager.SetSmearEffectActive(isSet);
    }
}
