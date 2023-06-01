using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerMouse_AudioVisualizer : MonoBehaviour
    , IAC_SystemAudio_RawSampleDataChangedHandler
{
    public Renderer rendererTarget;//Target to change "_Cutoff"

    #region Callback
    public void OnRawSampleDataChanged(float[] rawSampleData)
    {
        SetVisual(AC_ManagerHolder.SystemAudioManager.CalculateLoudness(rawSampleData));
    }

    private void SetVisual(float loudness)
    {
        rendererTarget.material.SetFloat("_Cutoff", 1 - loudness);
    }
    #endregion
}
