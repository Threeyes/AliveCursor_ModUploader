using System.Collections;
using System.Collections.Generic;
using Threeyes.Steamworks;
using UnityEngine;

public class StarWarsBB8_AudioVisualizer : MonoBehaviour
    , IHubSystemAudio_RawSampleDataChangedHandler
{
    public Transform tfTarget;
    public Vector3 maxOffset;

    #region Callback
    public void OnRawSampleDataChanged(float[] rawSampleData)
    {
        SetVisual(AudioVisualizerTool.CalculateLoudness(rawSampleData));
    }
    private void SetVisual(float loudness)
    {
        tfTarget.localPosition = Vector3.Lerp(Vector3.zero, maxOffset, loudness);
    }
    #endregion


}
