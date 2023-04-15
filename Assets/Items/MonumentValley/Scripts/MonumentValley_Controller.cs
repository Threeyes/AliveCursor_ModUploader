using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// 
/// Todo:
/// ָ��ͷ��
///     //-�̶�����ϵͳ������θı�ʱͬ����ɫ
/// ���ģ�
///     -����
/// ��ָ��ͷ��Ԫ���������ɶ�
/// </summary>
public class MonumentValley_Controller : MonoBehaviour
    , IAC_SystemAudio_SpectrumDataChangedHandler
{
    public bool ResponseToAudio { get { return responseToAudio; } set { responseToAudio = value; } }
    bool responseToAudio = true;
    public List<MonumentValley_UnitController> listUnitController = new List<MonumentValley_UnitController>();

    public void Offset(float percent)
    {
        foreach (var unitController in listUnitController)
        {
            unitController.Offset(percent);
        }
    }

    #region Callback
    public void OnSpectrumDataChanged(float[] data)
    {
        if (!responseToAudio)
            return;

        int unitCount = listUnitController.Count;
        if (unitCount > data.Length)
            return;
        for (int i = 0; i < unitCount; i++)
        {
            var unitController = listUnitController[i];
            int n = i * (data.Length / unitCount);
            float displacement = data[n];
            unitController.Offset(displacement);
        }
    }
    #endregion
}