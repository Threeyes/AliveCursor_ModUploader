using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Threeyes.Steamworks;
public class ComputerMouse_AudioVisualizer : MonoBehaviour
{
    public Renderer rendererTarget;//Target to change "_Cutoff"

    public void SetVisual(float loudness)
    {
        rendererTarget.material.SetFloat("_Cutoff", 1 - loudness);
    }
}
