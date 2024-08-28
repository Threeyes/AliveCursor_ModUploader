using System.Collections;
using System.Collections.Generic;
using Threeyes.GameFramework;
using UnityEngine;

public class ZhiYinHair_Controller : MonoBehaviour
        , IHubSystemAudio_RawSampleDataChangedHandler
{
    public AC_HairInstanceController hairInstanceController;

    #region Init
    protected virtual void OnEnable()
    {
        ManagerHolder.SystemAudioManager.Register(this);
    }
    protected virtual void OnDisable()
    {
        ManagerHolder.SystemAudioManager.UnRegister(this);
    }
    #endregion

    #region Callback
    float volume = 0;
    float lastVolume = 0;


    public bool IsRaiseHairWithAudio
    {
        get { return isRaiseHairWithAudio; }
        set
        {
            isRaiseHairWithAudio = value;
            if (!value)//禁用时需要重置
                SetHairPower(baseHairPower);
        }
    }
    public float RaiseHairPower { get { return raiseHairPower; } set { raiseHairPower = value; } }

    [SerializeField] private bool isRaiseHairWithAudio = true;
    [Range(10, 95)] private float raiseHairPower = 95;
    float baseHairPower = 5;
    public void OnRawSampleDataChanged(float[] data)
    {
        if (!IsRaiseHairWithAudio)
            return;

        if (data.Length < 3)
            return;

        ///功能：
        ///-基于音量调整头发升起幅度
        volume = AudioVisualizerTool.CalculateLoudness(data);
        if (volume != lastVolume)
        {
            SetHairPower(baseHairPower + RaiseHairPower * volume);//Raise hair with the rhythm [5,100]
            lastVolume = volume;
        }
    }

    void SetHairPower(float value)
    {
        hairInstanceController.Config.strandDiameter = Mathf.Clamp(value, 0.01F, 100);
        hairInstanceController.UpdateSetting();

    }
    #endregion
}
