using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShip_Controller : MonoBehaviour
{
    public Renderer rendererWindow;

    public void ChangeWindowTexture(Texture texture)
    {
        rendererWindow.material.mainTexture = texture;
    }
}
