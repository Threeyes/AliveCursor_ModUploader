using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteboardColorDisc : MonoBehaviour
{
    public Color color;
    public ColorEvent onSelect;

    private void OnTriggerEnter(Collider other)
    {
        WhiteboardPen whiteboardPen = other.GetComponentInParent<WhiteboardPen>();
        if(whiteboardPen)
        {
            whiteboardPen.SetPenColor(color);
        }
    }
}
