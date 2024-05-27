using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using com.zibra.liquid.DataStructures;
using com.zibra.liquid.Manipulators;
using com.zibra.liquid.Solver;
using Threeyes.Steamworks;

public class PhysicsLiquid_Controller : MonoBehaviour
    , IAC_ModHandler
    , IAC_CommonSetting_IsAliveCursorActiveHandler
    , IAC_CommonSetting_CursorSizeHandler
    , IAC_CursorState_ChangedHandler
    , IZibraLiquidController_SettingHandler
{
    public ZibraLiquid zibraLiquid;
    public ZibraLiquidSolverParameters zibraLiquidSolverParameters;
    public ZibraLiquidForceField zibraLiquidForceField;
    public Transform tfPosPivot;
    //Config
    public Vector2 viscosityRange = new Vector2(0f, 0.392f);
    public Vector2 fluidStiffnessRange = new Vector2(0f, 0.8f);
    public Vector2 forceFieldStrengthRange = new Vector2(0.01f, 1f);

    #region Callback
    public void OnModInit()
    {
    }
    public void OnModDeinit() { }

    void IZibraLiquidController_SettingHandler.UpdateSetting()
    {
        ///PS:
        ///-因为ZibraLiquidController会销毁因uMod打包导致重复添加的组件，因此要重新获取
        if (zibraLiquidSolverParameters == null)
            zibraLiquidSolverParameters = zibraLiquid.solverParameters;
    }

    public void OnIsAliveCursorActiveChanged(bool isActive)
    {
        SetZibraLiquidActive(isActive);
    }
    public void OnCursorSizeChanged(float value)
    {
        SetPosPivotPosition(value);
    }
    public void OnCursorStateChanged(AC_CursorStateInfo cursorStateInfo)
    {
        if (!zibraLiquidSolverParameters)
            return;

        //# ZibraLiquid
        //在隐藏相关State时，临时隐藏物体
        bool isHiding = cursorStateInfo.cursorState == AC_CursorState.Exit || cursorStateInfo.cursorState == AC_CursorState.Hide || cursorStateInfo.cursorState == AC_CursorState.StandBy;
        SetZibraLiquidActive(!isHiding);

        ///# ZibraLiquidForceField
        ///PS:
        ///-Viscosity代表粘度，影响液体的流动表现
        ///-FluidStiffness代表液体刚度，影响液体成型是否稳固
        if (cursorStateInfo.cursorState == AC_CursorState.Bored)
        {
            IncreaseFluidStiffnessAsync();//增加一个炸裂效果，形成散射
            zibraLiquidSolverParameters.Viscosity = viscosityRange.x;//粘度降低，类似水的行为
            zibraLiquidForceField.Strength = forceFieldStrengthRange.x;//减弱控制力
        }
        else
        {
            zibraLiquidSolverParameters.Viscosity = viscosityRange.y;//粘度调高，让其聚集在中心，避免移动时散布
            zibraLiquidForceField.Strength = forceFieldStrengthRange.y;
        }
    }
    void SetZibraLiquidActive(bool isActive)
    {
        zibraLiquid.gameObject.SetActive(isActive);
    }
    void SetPosPivotPosition(float cursorSize)
    {
        Transform tfAliveCursor = tfPosPivot.parent;
        float cursorUnitScale = 0.3f;
        float deltaPos = 0.35f;

        //PS：因为液体球的大小固定，因此要保证与光标保持一定距离
        tfPosPivot.position = tfAliveCursor.TransformPoint(new Vector3(0, -cursorUnitScale - deltaPos / cursorSize, 0));//因为Cursor被缩放，因此局部坐标要除以缩放值
    }

    async void IncreaseFluidStiffnessAsync()
    {
        zibraLiquidSolverParameters.FluidStiffness = fluidStiffnessRange.y;//临时更改刚度，便于打散
        await Task.Yield();
        zibraLiquidSolverParameters.FluidStiffness = fluidStiffnessRange.x;//恢复为默认值
    }
    #endregion

    #region Invoke by extern (CIB/CSB)
    public void OnCursorMouseDown(bool isDown)//Warning:不能为OnMouseDown，因为与父类同名方法冲突，导致uMod绑定错误
    {
        zibraLiquidSolverParameters.FluidStiffness = isDown ? fluidStiffnessRange.y : fluidStiffnessRange.x;//按下时打散以便重组
    }
    #endregion
}
