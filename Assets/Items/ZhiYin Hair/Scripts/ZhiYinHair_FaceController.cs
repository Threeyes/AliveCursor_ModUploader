using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhiYinHair_FaceController : MonoBehaviour
{
    public GifPlayer gifPlayer;//Warning����ΪAC_ImagePlayer��[RequireComponent]���Զ�����һ��GifPlayer��������Ҫָ����Ӧ�����
    public Renderer rendererFace;

    private void Start()
    {
        gifPlayer.onUpdateTexture.AddListener(UpdateTexture);
    }
    public void UpdateTexture(Texture texture)
    {
        rendererFace.material.SetTexture("_BaseMap", texture);
    }
}
