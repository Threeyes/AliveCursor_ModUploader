using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhiYinHair_Controller : MonoBehaviour
        , IAC_SystemAudio_RawSampleDataChangedHandler
{
    public AC_HairInstanceController hairInstanceController;
    #region Callback
    float volume = 0;
    float lastVolume = 0;
    public void OnRawSampleDataChanged(float[] data)
    {
        if (data.Length < 3)
            return;

        ///���ܣ�
        ///-�ƹ�����Ƶ��������
        ///-������ʱ�Ż���ת
        volume = AC_ManagerHolder.SystemAudioManager.CalculateLoudness(data);
    }


    private void Update()
    {
        if (volume != lastVolume)
        {

            lastVolume = volume;
        }
    }
    #endregion


}
