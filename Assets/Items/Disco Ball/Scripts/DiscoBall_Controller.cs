using Threeyes.Steamworks;
using UnityEngine;

public class DiscoBall_Controller : MonoBehaviour
    , IHubSystemAudio_RawSampleDataChangedHandler
{
    public Light lightRed;
    public Light lightGreen;
    public Light lightBlue;

    public Transform tfLightRig;
    public Transform tfDiscoBallRig;

    //Config
    public float BallRotateSpeed { get { return ballRotateSpeed; } set { ballRotateSpeed = value; } }
    [SerializeField] private float ballRotateSpeed = 5f;
    public float LightRotateSpeed { get { return lightRotateSpeed; } set { lightRotateSpeed = value; } }
    [SerializeField] private float lightRotateSpeed = 5f;

    #region Callback
    public void OnRawSampleDataChanged(float[] data)
    {
        if (data.Length < 3)
            return;

        ///���ܣ�
        ///-�ƹ�����Ƶ��������
        ///-������ʱ�Ż���ת
        volume = AC_ManagerHolder.SystemAudioManager.CalculateLoudness(data);
        Vector3 axisPercent = Vector3.zero;//ƫתʵ�֣�������ֵ�ֳ����ȷ֣��ֱ��ӦXYZ����ת����ֵ

        //data range: [-1.0, 1.0]
        int numPerSubArray = data.Length / 3;//ȡСֵ

        for (int i = 0; i != numPerSubArray; i++)
            axisPercent.x += data[i];
        for (int i = numPerSubArray; i != 2 * numPerSubArray; i++)
            axisPercent.y += data[i];
        for (int i = 2 * numPerSubArray; i != 3 * numPerSubArray; i++)
            axisPercent.z += data[i];
        axisPercent /= (numPerSubArray);
        if (volume > 0)
            axisPercent /= volume;//����������С��ɵ����˥��

        lightRed.intensity = axisPercent.x;
        lightGreen.intensity = axisPercent.y;
        lightBlue.intensity = axisPercent.z;
        curSpeed = Mathf.Lerp(curSpeed, (volume > 0) ? BallRotateSpeed : 0, Time.deltaTime);
    }
    #endregion
    float volume = 0;
    float curSpeed = 0;

    private void Update()
    {
        tfLightRig.Rotate(Vector3.up * lightRotateSpeed * Time.deltaTime, Space.Self);
        tfDiscoBallRig.Rotate(Vector3.up * curSpeed * Time.deltaTime, Space.Self);
    }
}
