using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// Ref:https://git.fh-aachen.de/MakeItTrue2/VR/-/blob/ea8b3e6728db29fd229603b12b3a5f88c315fa54/unity-game/Assets/Scripts/WhiteBoard/Whiteboard.cs
/// </summary>
public class Whiteboard : MonoBehaviour
{

    public int textureSize = 2048;
    private int penSize = 10;
    private Texture2D texture;
    private Color[] color;

    private bool touching, touchingLast;
    private float posX, posY;
    private float lastX, lastY;

    // Use this for initialization
    void Start()
    {
        // Set whiteboard texture
        Renderer renderer = GetComponent<Renderer>();
        texture = new Texture2D(textureSize, textureSize);
        texture.name = "Runtime Whiteboard";
        //Default is transparentti
        Color32[] resetColorArray = texture.GetPixels32();
        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = Color.clear;
        }
        texture.SetPixels32(resetColorArray);
        texture.Apply();

        renderer.material.mainTexture = texture;
    }



    // Update is called once per frame
    void Update()
    {
        // Transform textureCoords into "pixel" values
        int x = (int)(posX * textureSize - (penSize / 2));
        int y = (int)(posY * textureSize - (penSize / 2));

        x = Mathf.Clamp(x, 0, texture.width - 1);
        y = Mathf.Clamp(y, 0, texture.height - 1);

        // Only set the pixels if we were touching last frame
        if (touchingLast)
        {
            // Set base touch pixels
            texture.SetPixels(x, y, penSize, penSize, color);

            // Interpolate pixels from previous touch
            for (float t = 0.01f; t < 1.00f; t += 0.01f)
            {
                int lerpX = (int)Mathf.Lerp(lastX, (float)x, t);
                int lerpY = (int)Mathf.Lerp(lastY, (float)y, t);
                texture.SetPixels(lerpX, lerpY, penSize, penSize, color);
            }
        }

        // If currently touching, apply the texture
        if (touching)
        {
            texture.Apply();
        }

        this.lastX = x;
        this.lastY = y;
        this.touchingLast = this.touching;
    }

    public void ToggleTouch(bool touching)
    {
        this.touching = touching;
    }
    public void SetTouchPosition(float x, float y)
    {
        this.posX = x;
        this.posY = y;
    }
    public void SetColor(Color color)
    {
        this.color = Enumerable.Repeat<Color>(color, penSize * penSize).ToArray<Color>();
    }
    public void SetPenSize(int size)
    {
        penSize = size;
    }
}