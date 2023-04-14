using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonumentValley_ItemController : MonoBehaviour
{
    public Vector3 initLocalPos;


    [ContextMenu("SaveInitLocalPos")]
    public void SaveInitLocalPos()
    {
        initLocalPos = transform.localPosition;
    }
}
