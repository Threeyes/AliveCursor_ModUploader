using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
/// <summary>
/// Ref: https://git.fh-aachen.de/MakeItTrue2/VR/-/blob/ea8b3e6728db29fd229603b12b3a5f88c315fa54/unity-game/Assets/Scripts/WhiteBoard/WhiteboardPen.cs
/// 
/// Todo:
/// -提取配置
/// -滚轮
/// </summary>
public class WhiteboardPen : MonoBehaviour
    , IAC_SystemInput_MouseButtonHandler
    , IAC_SystemInput_MouseWheelHandler
{
    public Transform tfTip;//The end point of pen, the up axis must face the Whiteboard
    public Color penColor = Color.yellow;
    public int penSize = 2;

    [Header("Config")]
    public Vector2 penSizeRange = new Vector2(2, 10);

    [Header("Runtime")]
    public Whiteboard whiteboard;
    private RaycastHit touch;
    private Quaternion lastAngle;
    private bool lastTouch;

    #region Public
    public void SetPenColor(Color color)
    {
        penColor = color;
        //ToAdd:如果Tip有对应的Renderer，那就同步设置颜色
    }
    public void SetPenSize(int size)
    {
        PenSize = size;
    }
    #endregion

    #region Callback
    AC_MouseEventExtArgs curMouseEventExtArgs;

    public int PenSize
    {
        get
        {
            return penSize;
        }
        set
        {
            penSize = (int)Mathf.Clamp(value, penSizeRange.x, penSizeRange.y);
        }
    }

    public void OnMouseButton(AC_MouseEventExtArgs e)
    {
        curMouseEventExtArgs = e;

    }
    public void OnMouseWheel(AC_MouseEventExtArgs e)
    {
        if (e.WheelScrolled)
        {
            if (e.Delta > 0)
                PenSize += 1;
            else if (e.Delta < 0)
                PenSize -= 1;
        }
    }

    #endregion
    bool IsDrawKeyDown()
    {
        if (curMouseEventExtArgs == null)
            return false;
        //ToTest:看是否要检查Clicked
        return curMouseEventExtArgs.IsMouseButtonDown && curMouseEventExtArgs.Button == AC_MouseButtons.Middle;
    }
    bool IsDrawKeyUp()
    {
        if (curMouseEventExtArgs == null)
            return false;
        return curMouseEventExtArgs.IsMouseButtonUp && curMouseEventExtArgs.Button == AC_MouseButtons.Middle;
    }


    void Update()
    {
        //float tipHeight = transform.Find("Tip").transform.localScale.y;
        //Vector3 tip = transform.Find("Tip").transform.position;
        //Debug.Log(tip);
        //if (lastTouch)
        //{
        //    tipHeight *= 1.1f;
        //}

        // Check for a Raycast from the tip of the pen
        if (IsDrawKeyDown())
        {
            if (Physics.Raycast(tfTip.position, tfTip.up, out touch, 1000))
            {
                whiteboard = touch.collider.GetComponent<Whiteboard>();
                if (whiteboard == null) return;

                //// 【震动反馈】Give haptic feedback when touching the whiteboard
                //controllerActions.TriggerHapticPulse(0.05f);

                // Set whiteboard parameters
                whiteboard.SetPenSize(PenSize);
                whiteboard.SetColor(penColor);//画笔颜色
                whiteboard.SetTouchPosition(touch.textureCoord.x, touch.textureCoord.y);
                whiteboard.ToggleTouch(true);

                //// If we started touching, get the current angle of the pen
                //if (lastTouch == false)
                //{
                //    lastTouch = true;
                //    lastAngle = transform.rotation;
                //}
            }
        }
        else if (IsDrawKeyUp())
        {
            if (whiteboard)
            {
                whiteboard.ToggleTouch(false);
                whiteboard = null;
            }
            //lastTouch = false;
        }

        //// Lock the rotation of the pen if "touching"
        //if (lastTouch)
        //{
        //    transform.rotation = lastAngle;
        //}
    }

    #region Editor
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (tfTip)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(tfTip.position, tfTip.position + tfTip.up);
        }
    }
#endif
    #endregion

}
