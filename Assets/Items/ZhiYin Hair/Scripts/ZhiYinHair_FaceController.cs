using System.Collections;
using System.Collections.Generic;
using Threeyes.UI;
using UnityEngine;
/// <summary>
/// ToUpdate:
/// -ɾ�������ڸ���RendererHelper����
/// </summary>
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
