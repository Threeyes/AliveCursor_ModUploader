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

    //ToAdd:增加是否响应音频的开关
    public Vector3 rotationRange = new Vector3(5, 5, 5);


    #region Callback
    //ToAdd：随机控制脚的Weight（可以是通过offset的形式，这样能保证移动时不出错）
    public void OnRawSampleDataChanged(float[] data)
    {
        //PS:因为data range: [-1.0, 1.0]，刚好适用于正负随机旋转值
        if (data.Length < 3)
            return;

        int numPerSubArray = data.Length / 3;//取小值
        Vector3 rotatePercent = Vector3.zero;//偏转实现：将输入值分成三等分，分别对应XYZ的旋转缩放值
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
