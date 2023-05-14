using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 功能：
/// -当接收到音频信息时，播放跳舞动画
/// -当角色动画调用该类中的方法时，会自动发射蜘蛛网
/// </summary>
public class SpiderMan_Controller : MonoBehaviour
    , IAC_SystemAudio_RawSampleDataChangedHandler
{
    public Animator animator;
    public AC_ObjectMovement_FollowTarget objectMovement;
    public Transform tfWebShooter;//Z is the forward direction
    public Transform tfWebLineGroup;//Show/Hide all weblines
    public GameObject goPreWeb;

    [Header("Config")]
    public float webLineLifeTime = 2;//Lefetime for each webline

    [Header("Runtime")]
    public SpiderMan_WebLine curWebLine;


    private void Update()
    {
        //当角色准备减速
        if (curWebLine != null && objectMovement.CurMoveSpeedPercent < 1)
        {
            TryReleaseCurWebLine();
        }
    }
    #region Invoked by Swinging's AnimationEvent
    ///Todo:
    ///-蜘蛛丝是代物理引擎的线段（用插件实现），过几秒后自动销毁（这样可以在屏幕上看到很多残留的蜘蛛丝，而且有些还随机直接射到相机（屏幕）上）
    ///-检测到不在移动时应主动尝试释放，
    public void OnAniamtionSwinging_ShootWeb()
    {
        ////Todo:发射蜘蛛丝，并通过lineRenderer连接两个点
        SpiderMan_WebLine.ConfigInfo configInfo = new SpiderMan_WebLine.ConfigInfo()
        {
            tfWebShooter = tfWebShooter,
            lifeTime = webLineLifeTime
        };
        curWebLine = goPreWeb.InstantiatePrefab<SpiderMan_WebLine>(tfWebLineGroup);
        curWebLine.Init(configInfo);
    }

    public void OnAniamtionSwinging_ReleaseWeb()
    {
        TryReleaseCurWebLine();
    }

    private void TryReleaseCurWebLine()
    {
        curWebLine?.Release();
        curWebLine = null;
    }
    #endregion

    #region Callback
    float averageLoudness = 0;
    float lastSampleTime = 0;
    float samplingRate = 0.1f;
    public void OnRawSampleDataChanged(float[] rawSampleData)
    {
        //Bug：以下判断还是有问题，应该是取采样时间段内的平均值
        //Warning：要避免音频不连续导致动画停顿，需要判断采样时间段内的音量总值平均值（可能帧率设置太高导致采样不足，如音频采样率为60，但是刷新率为120就有问题（有数帧无数据导致音量为0），可以改为判断时间间隔而不是上次帧率）
        if (Time.time - lastSampleTime > samplingRate)
        {
            averageLoudness = 0;//Reset
        lastSampleTime = Time.time;
        }

        float loudness = AC_ManagerHolder.SystemAudioManager.CalculateLoudness(rawSampleData);
        averageLoudness += loudness;
        bool shouldDance = averageLoudness > 0;

        animator.SetBool("ShouldDance", shouldDance && objectMovement.CurMoveSpeed == 0);//Only dance on not moving and has audio input

        //lastLoudness = loudness;
    }
    #endregion

}
