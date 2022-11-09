using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerMouse_AudioVisualizer : MonoBehaviour
    //,IAC_SystemAudio_SpectrumDataChangedHandler
    , IAC_SystemAudio_RawSampleDataChangedHandler
{
    public Renderer rendererTarget;//Target to change "_Cutoff"


    private void LateUpdate()
    {
        if (!hasChangedInThisFrame)//Reset if no audio input
            SetVisual(0);
        hasChangedInThisFrame = false;//Reset
    }

    #region Callback
    bool hasChangedInThisFrame = false;
    public void OnRawSampleDataChanged(float[] rawSampleData)
    {
        //throw new System.NotImplementedException();
        //}
        //public void OnSpectrumDataChanged(float[] spectrumData)
        //{
        float loudness = AC_ManagerHolder.SystemAudioManager.CalculateLoudness(rawSampleData);

        SetVisual(loudness);
        hasChangedInThisFrame = true;//Mark as changed
    }

    private void SetVisual(float loudness)
    {
        rendererTarget.material.SetFloat("_Cutoff", 1 - loudness);
    }
    #endregion
}
