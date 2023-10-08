using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Threeyes.Steamworks;
public class ComputerMouse_AudioVisualizer : MonoBehaviour
    , IHubSystemAudio_RawSampleDataChangedHandler
{
    public Renderer rendererTarget;//Target to change "_Cutoff"

    #region Callback
    public void OnRawSampleDataChanged(float[] rawSampleData)
    {
        SetVisual(AudioVisualizerTool.CalculateLoudness(rawSampleData));
    }

    private void SetVisual(float loudness)
    {
        rendererTarget.material.SetFloat("_Cutoff", 1 - loudness);
    }
    #endregion
}
