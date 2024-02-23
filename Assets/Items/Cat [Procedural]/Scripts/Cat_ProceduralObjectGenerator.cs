using System.Collections;
using System.Collections.Generic;
using Threeyes.Core;
using Threeyes.Steamworks;
using UnityEngine;

public class Cat_ProceduralObjectGenerator : ObjectGenerator
    , IAC_CursorState_ChangedHandler
{
    public Transform tfContentParent;

    bool isHiding = false;
    public void OnCursorStateChanged(AC_CursorStateInfo cursorStateInfo)
    {
        isHiding = AC_ManagerHolder.StateManager.IsVanishState(cursorStateInfo.cursorState);
    }

    public void OnMouseDown(bool isDown)
    {
        if (isDown)
            Spawn();
    }

    protected void Spawn()
    {
        if (isHiding)
            return;
        if (AC_ManagerHolder.SystemCursorManager.CurSystemCursorAppearanceType == AC_SystemCursorAppearanceType.Arrow)
        {
            GameObject inst = GetPrefab()?.InstantiatePrefab(tfContentParent);
            inst.transform.position = transform.position;//重置到该物体的位置
        }
    }
}
