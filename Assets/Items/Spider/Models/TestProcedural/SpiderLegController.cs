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
/// 挪动操作（针对ChainIKConstraint：
/// -将Weight设置为0
/// -将Target的位置设置为落脚点
/// -将Weight设置为1（可以通过Tween）
/// 
/// 
/// Todo:
/// -也需要保存当前脚的固定点，这样即使挪动整个身体，脚也是固定在原地
/// -弄成局部坐标的长度（或者乘以SpiderGhostController的缩放）
/// </summary>
public class SpiderLegController : MonoBehaviour
{
    public ChainIKConstraint chainIKConstraint;
    public Transform tfFoothold;//落脚点

    public float moveFootDistance = 0.1f;//当脚与落脚点的距离超过一定距离后更新脚位置
    public float tweenDuration = 0.1f;

    [Header("Runtime")]
    public Transform tfSourceTarget;//运行时从chainIKConstraint中获取，注意要与模型分开摆放，否则会受其位置影响
    public float curDistance;

    public bool NeedMove { get { return isExcessive && !isTweening; } }
    public bool isExcessive = false;//距离过长
    public bool isTweening = false;
    public Vector3 targetPos;
    private void Awake()
    {
        tfSourceTarget = chainIKConstraint.data.target;
        tfFoothold.position = tfSourceTarget.position = chainIKConstraint.data.tip.position;//同步初始位置（为了方便同步，chainIKConstraint的默认Weight可以设置为0）

        targetPos = tfFoothold.position;
    }
    private void Update()
    {
        curDistance = Vector3.Distance(tfSourceTarget.position, tfFoothold.position);

        isExcessive = curDistance > moveFootDistance;
        //if (isExcessive)
        //{
        //    TweenMoveAsync();
        //}
    }

    public Ease easeExit = Ease.Linear;
    public Ease easeEnter = Ease.Linear;
    public async void TweenMoveAsync()
    {
        if (!isExcessive)//不需要判断，强制更新位置
            return;

        isTweening = true;

        //PS:因为脚长有限，因此新位置只能是与目标连线的投影（长度为moveFootDistance）位置
        Vector3 maxVector = (tfFoothold.position - tfSourceTarget.position).normalized;
        maxVector.Scale(Vector3.one * moveFootDistance);
        targetPos = tfSourceTarget.position + maxVector;

        var tweenExit = DOTween.To(() => chainIKConstraint.weight, (val) => chainIKConstraint.weight = val, 0, tweenDuration / 2).SetEase(easeExit);
        tweenExit.onComplete +=
            () =>
            {
                var tweenEnter = DOTween.To(() => chainIKConstraint.weight, (val) => chainIKConstraint.weight = val, 1, tweenDuration / 2).SetEase(easeEnter);
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

    [Header("Editor")]
    public float gizmosRadius = 0.1f;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(tfFoothold.position, gizmosRadius);
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(targetPos, gizmosRadius);

        if (Application.isPlaying && tfSourceTarget)
        {
            float distancePercent = Vector3.Distance(targetPos, tfSourceTarget.position) / moveFootDistance;
            Color color = Color.Lerp(Color.green, Color.red, distancePercent);
            Gizmos.color = color;
            Gizmos.DrawLine(targetPos, tfSourceTarget.position);
        }
    }
}
