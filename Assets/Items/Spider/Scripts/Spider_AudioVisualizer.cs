using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Response To Audio
/// </summary>
public class Spider_AudioVisualizer : MonoBehaviour
    , IAC_SystemAudio_RawSampleDataChangedHandler
{
    public CreeperGhostController creeperGhostController;

    //ToAdd:�����Ƿ���Ӧ��Ƶ�Ŀ���
    public Vector3 rotationRange = new Vector3(5, 5, 5);


    #region Callback
    //ToAdd��������ƽŵ�Weight��������ͨ��offset����ʽ�������ܱ�֤�ƶ�ʱ������
    public void OnRawSampleDataChanged(float[] data)
    {
        //PS:��Ϊdata range: [-1.0, 1.0]���պ����������������תֵ
        if (data.Length < 3)
            return;

        int numPerSubArray = data.Length / 3;//ȡСֵ
        Vector3 rotatePercent = Vector3.zero;//ƫתʵ�֣�������ֵ�ֳ����ȷ֣��ֱ��ӦXYZ����ת����ֵ
        for (int i = 0; i != numPerSubArray; i++)
            rotatePercent.x += data[i];
        for (int i = numPerSubArray; i != 2 * numPerSubArray; i++)
            rotatePercent.y += data[i];
        for (int i = 2 * numPerSubArray; i != 3 * numPerSubArray; i++)
            rotatePercent.z += data[i];
        rotatePercent /= numPerSubArray;

        creeperGhostController.tfGhostBody.localEulerAngles = rotationRange.Multi(rotatePercent);
    }
    #endregion
}
