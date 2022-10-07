using System;
using System.Collections;
using System.Collections.Generic;
using Threeyes.Persistent;
using UnityEngine;
using UnityEngine.Animations.Rigging;
/// <summary>
/// Response To Audio
/// </summary>
public class Spider_AudioVisualizer : MonoBehaviour
    , IAC_SystemAudio_RawSampleDataChangedHandler
{
    //ToUpdate:等PersistentData_Object完善后改成其方案
    public bool CanBodyMove { get { return config.canBodyMove; } set { config.canBodyMove = value; } }
    public Vector3 BodyMoveRange { get { return config.bodyMoveRange; } set { config.bodyMoveRange = value; } }
    public bool CanBodyRotate { get { return config.canBodyRotate; } set { config.canBodyRotate = value; } }
    public Vector3 BodyRotateRange { get { return config.bodyRotateRange; } set { config.bodyRotateRange = value; } }
    public bool CanLegRaise { get { return config.canLegRaise; } set { config.canLegRaise = value; } }
    public ConfigInfo.LegType LegToRaise { get { return config.legToRaise; } set { config.legToRaise = value; } }//Todo:不行就用int转枚举
    public float LegRaiseRange { get { return config.legRaiseRange; } set { config.legRaiseRange = value; } }

    public CreeperGhostController creeperGhostController;
    public List<CreeperLegGhostController> listCreeperLegGhostController = new List<CreeperLegGhostController>();//Follow enum LegType's order
    [SerializeField] protected ConfigInfo config;

    private void LateUpdate()
    {
        //Reset if no audio input
        if (!hasChangedInThisFrame)
        {
            creeperGhostController.tfGhostBody.localPosition = Vector3.zero;
            creeperGhostController.tfGhostBody.localEulerAngles = Vector3.zero;
            listCreeperLegGhostController.ForEach(c =>
            {
                if (!c.isMoving)
                    c.CompWeight = 1;
            });
        }
        hasChangedInThisFrame = false;//Reset
    }

    #region Callback
    public void SetLegToRaise(int index)//通过PD调用，从1开始
    {
        ConfigInfo.LegType legToRaise = ConfigInfo.LegType.None;
        if (index == 1)
            legToRaise = ConfigInfo.LegType.FrontLeft;
        else if (index == 2)
            legToRaise = ConfigInfo.LegType.FrontRight;
        else if (index == 3)
            legToRaise = ConfigInfo.LegType.BackLeft;
        else if (index == 4)
            legToRaise = ConfigInfo.LegType.BackRight;
        LegToRaise = legToRaise;
    }

    public ChainIKConstraint TestChainIKConstraint;
    //ToAdd：随机控制脚的Weight（可以是通过offset的形式，这样能保证移动时不出错）

    /// <summary>
    /// PS:
    /// -只有当前有音频数据时才会进入
    /// </summary>
    /// <param name="data"></param>
    //public void OnFFTDataChanged(float[] data)
    //{
    bool hasChangedInThisFrame = false;
    public void OnRawSampleDataChanged(float[] data)
    {
        float volume = AC_ManagerHolder.SystemAudioManager.CalculateLoudness(data);
        Vector3 axisPercent = Vector3.zero;//偏转实现：将输入值分成三等分，分别对应XYZ的旋转缩放值

        //PS:因为data range: [-1.0, 1.0]，刚好适用于正负随机旋转值
        if (data.Length < 3)
            return;
        int numPerSubArray = data.Length / 3;//取小值

        for (int i = 0; i != numPerSubArray; i++)
            axisPercent.x += data[i];
        for (int i = numPerSubArray; i != 2 * numPerSubArray; i++)
            axisPercent.y += data[i];
        for (int i = 2 * numPerSubArray; i != 3 * numPerSubArray; i++)
            axisPercent.z += data[i];
        //Debug.Log(rotatePercent / numPerSubArray + "/" + volume+"="+ rotatePercent / numPerSubArray/ volume);
        axisPercent /= (numPerSubArray);
        if (volume > 0)
            axisPercent /= volume;//消除音量大小造成的振幅衰减

        //Body
        creeperGhostController.tfGhostBody.localPosition = config.canBodyMove ? config.bodyMoveRange.Multi(axisPercent) : Vector3.zero;
        creeperGhostController.tfGhostBody.localEulerAngles = config.canBodyRotate ? config.bodyRotateRange.Multi(axisPercent) : Vector3.zero;

        //Todo：只有任意脚不在移动时才能更改该值
        //Legs
        var listDesireLegController = GetDesireLegControllers();
        listCreeperLegGhostController.ForEach(c =>
        {
            if (!c.isMoving)
            {
                c.CompWeight = listDesireLegController.Contains(c) ? volume * config.legRaiseRange + (1 - config.legRaiseRange) : 1;//volume reach max== Weight is 1 (模拟随着节拍跺脚，音量最大对应脚落下)
            }
        });

        hasChangedInThisFrame = true;//Mark as changed
    }

    List<CreeperLegGhostController> GetDesireLegControllers()
    {
        List<CreeperLegGhostController> listController = new List<CreeperLegGhostController>();
        if (config.canLegRaise)
        {
            var legType = config.legToRaise;
            if (legType.Has(ConfigInfo.LegType.FrontLeft))
                listController.Add(listCreeperLegGhostController[0]);
            if (legType.Has(ConfigInfo.LegType.FrontRight))
                listController.Add(listCreeperLegGhostController[1]);
            if (legType.Has(ConfigInfo.LegType.BackLeft))
                listController.Add(listCreeperLegGhostController[2]);
            if (legType.Has(ConfigInfo.LegType.BackRight))
                listController.Add(listCreeperLegGhostController[3]);
        }
        return listController;
    }
    #endregion

    #region Define
    /// <summary>
    /// 
    /// ToUpdate:
    /// -使用Callback，避免频繁判断及获取Leg等操作，以及在切换Leg后重置其他Leg
    /// </summary>
    [Serializable]
    public class ConfigInfo : AC_SerializableDataBase
    {
        //ToAdd:分别为Move和Rotate增加是否响应音频的开关
        public bool canBodyMove = true;
        public Vector3 bodyMoveRange = new Vector3(1, 1, 0);
        public bool canBodyRotate = false;
        public Vector3 bodyRotateRange = new Vector3(5, 0, 5);
        public bool canLegRaise = false;
        public LegType legToRaise = LegType.FrontLeft;
        [Range(0.1f, 1)] public float legRaiseRange = 0.5f;

        [Flags]
        public enum LegType//PS:因为每个脚的数量都不一样，因此由Modder自行定义枚举
        {
            None = 0,

            FrontLeft = 1 << 0,
            FrontRight = 1 << 1,
            BackLeft = 1 << 2,
            BackRight = 1 << 3,

            All = ~0//-1
        }
    }
    #endregion
}
