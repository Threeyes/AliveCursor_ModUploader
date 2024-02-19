using System.Collections;
using System.Collections.Generic;
using Threeyes.Steamworks;
using UnityEngine;

public class StarWarsBB8_AudioVisualizer : MonoBehaviour
    , IHubSystemAudio_RawSampleDataChangedHandler
{
    public Transform tfTarget;
    public Vector3 maxOffset;

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
