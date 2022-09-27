using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System.Threading.Tasks;
/// <summary>
/// 功能：
/// -标注Spider单个脚的落脚点，某个脚与落脚点的距离过大时，就开始挪动操作
/// 
/// Todo:
/// -也需要保存当前脚的固定点，这样即使挪动整个身体，脚也是固定在原地
/// -不要跟踪对应GhostLeg目标，而是当父物体偏移达到一定值就自动移动，这样能解决Bored时快速旋转导致脚扭成一团的问题
/// -弄成局部坐标的长度（或者乘以SpiderGhostController的缩放）
/// </summary>
public class CreeperLegGhostController : ComponentHelperBase<ChainIKConstraint>
        , IAC_CommonSetting_CursorSizeHandler
{
    public bool NeedMove { get { return isExcessive && !isTweening; } }
    public float CompWeight { get { return Comp.weight; } set { Comp.weight = value; } }

    public Transform tfGhostLeg;//脚的目标点
    public float legMaxDistance = 0.1f;//脚能移动的最远距离(当脚与落脚点的距离超过一定距离后更新脚位置)
    public Vector2 weightRange = new Vector2(0, 1);
    public float tweenDuration = 0.1f;
    public Ease easeLegUp = Ease.Linear;
    public Ease easeLegDown = Ease.Linear;

    [Header("Runtime")]
    public Transform tfSourceTarget;//运行时从chainIKConstraint中获取，注意要与模型分开摆放，否则会受其位置影响
    public float curDistance;
    public bool isExcessive = false;//距离过长
    public bool isTweening = false;
    public Vector3 targetPos;


    private void Awake()
    {
        tfSourceTarget = Comp.data.target;
        tfGhostLeg.position = tfSourceTarget.position = Comp.data.tip.position;//同步初始位置（为了方便同步，chainIKConstraint的默认Weight可以设置为0）

        targetPos = tfGhostLeg.position;
        settingCursorSize = AC_ManagerHolder.CommonSettingManager.CursorSize;
    }
    public float LegMaxDistanceFinal { get { return legMaxDistance * settingCursorSize; } }//乘以光标的缩放值

    private void Update()
    {
        curDistance = Vector3.Distance(tfSourceTarget.position, tfGhostLeg.position);

        isExcessive = curDistance > LegMaxDistanceFinal;
        //if (isExcessive)
        //{
        //    TweenMoveAsync();
        //}
    }

    public async void TweenMoveAsync(bool forceUpdate = false)
    {
        if (!isExcessive)//不需要判断，强制更新位置
            return;

        isTweening = true;
       
        /// 挪动操作（针对ChainIKConstraint：
        /// -将Weight设置为0
        /// -将Target的位置设置为落脚点
        /// -将Weight设置为1（可以通过Tween）
        /// PS:
        /// -因为脚长有限，因此新位置只能是与目标连线的投影（长度为moveFootDistance）位置
        Vector3 vector = tfGhostLeg.position - tfSourceTarget.position;
        Vector3 vectorNormal = vector.normalized;
        vectorNormal.Scale(Vector3.one * Mathf.Min(vector.magnitude, LegMaxDistanceFinal));
        targetPos = tfSourceTarget.position + vectorNormal;

        var tweenExit = DOTween.To(() => CompWeight, (val) => CompWeight = val, weightRange.x, tweenDuration / 2).SetEase(easeLegUp);
        tweenExit.onComplete +=
            () =>
            {
                var tweenEnter = DOTween.To(() => CompWeight, (val) => CompWeight = val, weightRange.y, tweenDuration / 2).SetEase(easeLegDown);
                tweenEnter.onComplete +=
                () =>
                {
                    tfSourceTarget.position = targetPos;
                    isTweening = false;
                };
            };

        while (!isTweening)
        {
            await Task.Yield();
        }
    }

    #region Callback
    public float settingCursorSize = 1;
    public void OnCursorSizeChanged(float value)
    {
        settingCursorSize = value;
    }
    #endregion

    [Header("Editor")]
    public float gizmosRadius = 0.1f;
    private void OnDrawGizmos()
    {
        if (tfGhostLeg)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(tfGhostLeg.position, gizmosRadius);
            Gizmos.color = Color.white;
        }

        Gizmos.DrawWireSphere(targetPos, gizmosRadius);

        if (Application.isPlaying && tfSourceTarget)
        {
            float distancePercent = Vector3.Distance(targetPos, tfSourceTarget.position) / LegMaxDistanceFinal;
            Color color = Color.Lerp(Color.green, Color.red, distancePercent);
            Gizmos.color = color;
            Gizmos.DrawLine(targetPos, tfSourceTarget.position);
        }
    }
}
