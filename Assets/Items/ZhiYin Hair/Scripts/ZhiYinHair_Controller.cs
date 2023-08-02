using System.Collections;
using System.Collections.Generic;
using Threeyes.Steamworks;
using UnityEngine;

public class ZhiYinHair_Controller : MonoBehaviour
        , IHubSystemAudio_RawSampleDataChangedHandler
{
    public AC_HairInstanceController hairInstanceController;
    #region Callback
    float volume = 0;
    float lastVolume = 0;


    public bool IsRaiseHairWithAudio { get { return isRaiseHairWithAudio; } set { isRaiseHairWithAudio = value; } }
    public float RaiseHairPower { get { return raiseHairPower; } set { raiseHairPower = value; } }

    [SerializeField] private bool isRaiseHairWithAudio = true;
    [Range(10, 95)] private float raiseHairPower = 95;
    public void OnRawSampleDataChanged(float[] data)
    {
        if (!IsRaiseHairWithAudio)
            return;

        if (data.Length < 3)
            return;

        ///功能：
        ///-基于音量调整头发升起幅度
        volume = AC_ManagerHolder.SystemAudioManager.CalculateLoudness(data);
        if (volume != lastVolume)
        {
            hairInstanceController.Config.strandDiameter = 5 + RaiseHairPower * volume;//Raise hair with the rhythm [5,100]
                                                                                       //hairInstanceController.Config.strandMargin = 5 + 95 * volume;//Raise hair with the rhythm [5,100]
            hairInstanceController.UpdateSetting();
            lastVolume = volume;
        }
    }

    #endregion
}
