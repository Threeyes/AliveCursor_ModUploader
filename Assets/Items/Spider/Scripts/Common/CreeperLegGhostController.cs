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
/// -脚：
///     -！！活动范围应该是以根关节与躯干的中点为原点，半径为legMaxDistance的球形区域，然后是基于当前躯体的相对位置为轴心，移动时优先移动targetPos跟中点比重最近的脚（可通过计算连线跟球体的交点，取最短值）
/// </summary>
public class CreeperLegGhostController : ComponentHelperBase<ChainIKConstraint>
        , IAC_CommonSetting_CursorSizeHandler
{
    //ToAdd:限制可移动区域为原点的指定圆形区间（在TweenMoveAsync中判断是否可以移动）
    public bool NeedMove { get { return isExcessive && !isMoving; } }
    public float CompWeight { get { return Comp.weight; } set { Comp.weight = value; } }
    public float MaxReachDistanceFinal { get { return maxReachDistance * settingCursorSize; } }//乘以光标缩放值
    public float UpdatePositionDistanceFinal { get { return updatePositionDistance * settingCursorSize; } }

    public Transform tfGhostLeg;//脚的目标点
    public float updatePositionDistance = 0.1f;//检查是否需要移动(当脚与目标点的距离超过一定距离后更新脚位置)
    public float maxReachDistance = 0.3f;//脚能移动的最远距离
    public Vector2 weightRange = new Vector2(0, 1);
    public float tweenDuration = 0.08f;
    public Ease easeLegUp = Ease.Linear;
    public Ease easeLegDown = Ease.Linear;

    [Header("Runtime")]
    public Transform tfSourceTarget;//运行时从chainIKConstraint中获取，注意要与模型分开摆放，否则会受其位置影响
    public float curDistance;
    public bool isExcessive = false;//距离过长（需要移动）
    public bool isMoving = false;//正在移动
    public Vector3 targetPos;

    //Pivot
    CreeperGhostControllerManager creeperGhostController;
    public Transform tfModelRoot;
    public Vector3 localPivotPos;//脚移动的轴心（相对于躯干的局部位置，注意因为缩放同步，因此不需要乘以光标尺寸）
    private void Awake()
    {
        tfSourceTarget = Comp.data.target;
        tfGhostLeg.position = tfSourceTarget.position = Comp.data.tip.position;//同步初始位置

        targetPos = tfGhostLeg.position;
        settingCursorSize = AC_ManagerHolder.CommonSettingManager.CursorSize;

        //以脚末端及躯干的中点作为脚的锚点
        creeperGhostController = transform.GetComponentInParent<CreeperGhostControllerManager>(true);
        tfModelRoot = creeperGhostController.tfModelRoot;
        localPivotPos = tfModelRoot.InverseTransformPoint((tfModelRoot.position + tfSourceTarget.position) / 2);
    }

    private void Update()
    {
        curDistance = Vector3.Distance(tfSourceTarget.position, tfGhostLeg.position);
        isExcessive = curDistance > UpdatePositionDistanceFinal;
        //if (isExcessive)
        //{
        //    TweenMoveAsync();
        //}
    }

    public async void TweenMoveAsync(bool forceUpdate = false)
    {
        if (!forceUpdate)
            if (!isExcessive)//不需要判断，强制更新位置
                return;

        isMoving = true;

        ///-计算目的点与锚点的连线与半径范围球体的交点（如果在球体内，则直接使用目的点），然后取最靠近目的地的点
        /// PS:
        /// -因为脚长有限，因此新位置只能是与目标连线的投影（长度为moveFootDistance）位置
        Vector3 worldPivotPos = tfModelRoot.TransformPoint(localPivotPos);
        Vector3 vector = tfGhostLeg.position - worldPivotPos;
        float vectorLength = vector.magnitude;

        //Todo:检查targetPos是否在可移动范围中
        if ((vectorLength - MaxReachDistanceFinal) < 0)//在可移动区域内:直接使用目标位置
        {
            targetPos = tfGhostLeg.position;
        }
        else//在可移动区域外：使用连线最远点
        {
            Vector3 vectorNormal = vector.normalized;
            vectorNormal.Scale(Vector3.one * MaxReachDistanceFinal);
            targetPos = worldPivotPos + vectorNormal;
        }

        /// 挪动操作（针对ChainIKConstraint：
        /// -将Weight设置为0
        /// -将Target的位置设置为落脚点
        /// -将Weight设置为1（可以通过Tween）
        var tweenExit = DOTween.To(() => CompWeight, (val) => CompWeight = val, weightRange.x, tweenDuration / 2).SetEase(easeLegUp);
        tweenExit.onComplete +=
            () =>
            {
                var tweenEnter = DOTween.To(() => CompWeight, (val) => CompWeight = val, weightRange.y, tweenDuration / 2).SetEase(easeLegDown);
                tweenEnter.onComplete +=
                () =>
                {
                    tfSourceTarget.position = targetPos;
                    isMoving = false;
                };
            };

        while (!isMoving)
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
        //Pivot
        if (tfModelRoot)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(tfModelRoot.TransformPoint(localPivotPos), gizmosRadius);
            Gizmos.color = Color.white;
        }

        if (tfGhostLeg)//绿色代表Ghost
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(tfGhostLeg.position, gizmosRadius);
            Gizmos.color = Color.white;
        }

        Gizmos.DrawWireSphere(targetPos, gizmosRadius);//白色代表目标位置

        if (Application.isPlaying && tfSourceTarget)//渐变色代表距离接近度
        {
            float distancePercent = Vector3.Distance(targetPos, tfSourceTarget.position) / MaxReachDistanceFinal;
            Color color = Color.Lerp(Color.green, Color.red, distancePercent);
            Gizmos.color = color;
            Gizmos.DrawLine(targetPos, tfSourceTarget.position);
        }
    }
}
