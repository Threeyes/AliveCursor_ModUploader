using System;
using System.Collections;
using System.Collections.Generic;
using Threeyes.Persistent;
using UnityEngine;
using UnityEngine.Animations.Rigging;
/// <summary>
/// Response To Audio
/// 
/// 功能：
/// 通过序号确定指定响应音频的Leg
/// </summary>
public class Creeper_AudioVisualizer : MonoBehaviour
    , IAC_SystemAudio_RawSampleDataChangedHandler
{
    public bool CanBodyMove { get { return config.canBodyMove; } set { config.canBodyMove = value; } }
    public Vector3 BodyMoveRange { get { return config.bodyMoveRange; } set { config.bodyMoveRange = value; } }
    public bool CanBodyRotate { get { return config.canBodyRotate; } set { config.canBodyRotate = value; } }
    public Vector3 BodyRotateRange { get { return config.bodyRotateRange; } set { config.bodyRotateRange = value; } }
    public bool CanLegRaise { get { return config.canLegRaise; } set { config.canLegRaise = value; } }
    public int RaiseLegIndex { get { return config.raiseLegIndex; } set { config.raiseLegIndex = value; } }
    public float LegRaiseRange { get { return config.legRaiseRange; } set { config.legRaiseRange = value; } }

    public CreeperGhostController creeperGhostController;
    public List<CreeperLegGhostController> listCreeperLegGhostController { get { return creeperGhostController.ListComp; } }
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

        ///Body
        ///-Sync with rhythm
        creeperGhostController.tfGhostBody.localPosition = config.canBodyMove ? config.bodyMoveRange.Multi(axisPercent) : Vector3.zero;
        creeperGhostController.tfGhostBody.localEulerAngles = config.canBodyRotate ? config.bodyRotateRange.Multi(axisPercent) : Vector3.zero;

        ///Legs       
        ///-Raise Leg
        var legControllerToRaise = GetLegControllerToRaise();//获取需要抬脚的Controller
        listCreeperLegGhostController.ForEach(c =>
        {
            if (!c.isMoving)//只有脚不移动时才能更改该值
            {
                //c.CompWeight = listDesireLegController.Contains(c) ? volume * config.legRaiseRange + (1 - config.legRaiseRange) : 1;//volume reach max== Weight is 1 (模拟随着节拍跺脚，音量最大对应脚落下。缺点是暂停播放时脚一直抬着，弃用。)

                c.CompWeight = legControllerToRaise == c ? 1 - volume * config.legRaiseRange : 1;//volume reach min== Weight is 1
            }
        });

        hasChangedInThisFrame = true;//Mark as changed
    }

    CreeperLegGhostController GetLegControllerToRaise()
    {
        if (CanLegRaise)
        {
            if (RaiseLegIndex >= 0 && RaiseLegIndex < listCreeperLegGhostController.Count)
            {
                return listCreeperLegGhostController[RaiseLegIndex];
            }
        }
        return null;
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
        public int raiseLegIndex = 0;//Which leg to raise
        [Range(0.1f, 1)] public float legRaiseRange = 0.5f;
    }
    #endregion
}
