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
    //ToUpdate:��PersistentData_Object���ƺ�ĳ��䷽��
    public bool CanBodyMove { get { return config.canBodyMove; } set { config.canBodyMove = value; } }
    public Vector3 BodyMoveRange { get { return config.bodyMoveRange; } set { config.bodyMoveRange = value; } }
    public bool CanBodyRotate { get { return config.canBodyRotate; } set { config.canBodyRotate = value; } }
    public Vector3 BodyRotateRange { get { return config.bodyRotateRange; } set { config.bodyRotateRange = value; } }
    public bool CanLegRaise { get { return config.canLegRaise; } set { config.canLegRaise = value; } }
    public ConfigInfo.LegType LegToRaise { get { return config.legToRaise; } set { config.legToRaise = value; } }//Todo:���о���intתö��
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
    public void SetLegToRaise(int index)//ͨ��PD���ã���1��ʼ
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
    //ToAdd��������ƽŵ�Weight��������ͨ��offset����ʽ�������ܱ�֤�ƶ�ʱ������

    /// <summary>
    /// PS:
    /// -ֻ�е�ǰ����Ƶ����ʱ�Ż����
    /// </summary>
    /// <param name="data"></param>
    //public void OnFFTDataChanged(float[] data)
    //{
    bool hasChangedInThisFrame = false;
    public void OnRawSampleDataChanged(float[] data)
    {
        float volume = AC_ManagerHolder.SystemAudioManager.CalculateLoudness(data);
        Vector3 axisPercent = Vector3.zero;//ƫתʵ�֣�������ֵ�ֳ����ȷ֣��ֱ��ӦXYZ����ת����ֵ

        //PS:��Ϊdata range: [-1.0, 1.0]���պ����������������תֵ
        if (data.Length < 3)
            return;
        int numPerSubArray = data.Length / 3;//ȡСֵ

        for (int i = 0; i != numPerSubArray; i++)
            axisPercent.x += data[i];
        for (int i = numPerSubArray; i != 2 * numPerSubArray; i++)
            axisPercent.y += data[i];
        for (int i = 2 * numPerSubArray; i != 3 * numPerSubArray; i++)
            axisPercent.z += data[i];
        //Debug.Log(rotatePercent / numPerSubArray + "/" + volume+"="+ rotatePercent / numPerSubArray/ volume);
        axisPercent /= (numPerSubArray);
        if (volume > 0)
            axisPercent /= volume;//����������С��ɵ����˥��

        //Body
        creeperGhostController.tfGhostBody.localPosition = config.canBodyMove ? config.bodyMoveRange.Multi(axisPercent) : Vector3.zero;
        creeperGhostController.tfGhostBody.localEulerAngles = config.canBodyRotate ? config.bodyRotateRange.Multi(axisPercent) : Vector3.zero;

        //Todo��ֻ������Ų����ƶ�ʱ���ܸ��ĸ�ֵ
        //Legs
        var listDesireLegController = GetDesireLegControllers();
        listCreeperLegGhostController.ForEach(c =>
        {
            if (!c.isMoving)
            {
                c.CompWeight = listDesireLegController.Contains(c) ? volume * config.legRaiseRange + (1 - config.legRaiseRange) : 1;//volume reach max== Weight is 1 (ģ�����Ž��Ķ�ţ���������Ӧ������)
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
    /// -ʹ��Callback������Ƶ���жϼ���ȡLeg�Ȳ������Լ����л�Leg����������Leg
    /// </summary>
    [Serializable]
    public class ConfigInfo : AC_SerializableDataBase
    {
        //ToAdd:�ֱ�ΪMove��Rotate�����Ƿ���Ӧ��Ƶ�Ŀ���
        public bool canBodyMove = true;
        public Vector3 bodyMoveRange = new Vector3(1, 1, 0);
        public bool canBodyRotate = false;
        public Vector3 bodyRotateRange = new Vector3(5, 0, 5);
        public bool canLegRaise = false;
        public LegType legToRaise = LegType.FrontLeft;
        [Range(0.1f, 1)] public float legRaiseRange = 0.5f;

        [Flags]
        public enum LegType//PS:��Ϊÿ���ŵ���������һ���������Modder���ж���ö��
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
