using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhiYinHair_FaceController : MonoBehaviour
{
    public GifPlayer gifPlayer;//Warning：因为AC_ImagePlayer的[RequireComponent]会自动增加一个GifPlayer组件，因此要指定对应的组件
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
