using System;
using System.Collections;
using System.Collections.Generic;
using Threeyes.Persistent;
using UnityEngine;
using UnityEngine.Animations.Rigging;
/// <summary>
/// Response To Audio
/// 
/// ���ܣ�
/// ͨ�����ȷ��ָ����Ӧ��Ƶ��Leg
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

        ///Body
        ///-Sync with rhythm
        creeperGhostController.tfGhostBody.localPosition = config.canBodyMove ? config.bodyMoveRange.Multi(axisPercent) : Vector3.zero;
        creeperGhostController.tfGhostBody.localEulerAngles = config.canBodyRotate ? config.bodyRotateRange.Multi(axisPercent) : Vector3.zero;

        ///Legs       
        ///-Raise Leg
        var legControllerToRaise = GetLegControllerToRaise();//��ȡ��Ҫ̧�ŵ�Controller
        listCreeperLegGhostController.ForEach(c =>
        {
            if (!c.isMoving)//ֻ�нŲ��ƶ�ʱ���ܸ��ĸ�ֵ
            {
                //c.CompWeight = listDesireLegController.Contains(c) ? volume * config.legRaiseRange + (1 - config.legRaiseRange) : 1;//volume reach max== Weight is 1 (ģ�����Ž��Ķ�ţ���������Ӧ�����¡�ȱ������ͣ����ʱ��һֱ̧�ţ����á�)

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
        public int raiseLegIndex = 0;//Which leg to raise
        [Range(0.1f, 1)] public float legRaiseRange = 0.5f;
    }
    #endregion
}
