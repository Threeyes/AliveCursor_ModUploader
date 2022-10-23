using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using System.Threading.Tasks;
using System.Linq;
/// <summary>
/// ���ܣ�
/// -��עSpider�����ŵ���ŵ㣬ĳ��������ŵ�ľ������ʱ���Ϳ�ʼŲ������
/// 
/// Todo:
/// -Ҳ��Ҫ���浱ǰ�ŵĹ̶��㣬������ʹŲ���������壬��Ҳ�ǹ̶���ԭ��
/// -��Ҫ���ٶ�ӦGhostLegĿ�꣬���ǵ�������ƫ�ƴﵽһ��ֵ���Զ��ƶ��������ܽ��Boredʱ������ת���½�Ť��һ�ŵ�����
/// -Ū�ɾֲ�����ĳ��ȣ����߳���SpiderGhostController�����ţ�
/// -�ţ�
///     -�������ΧӦ�����Ը��ؽ������ɵ��е�Ϊԭ�㣬�뾶ΪlegMaxDistance����������Ȼ���ǻ��ڵ�ǰ��������λ��Ϊ���ģ��ƶ�ʱ�����ƶ�targetPos���е��������Ľţ���ͨ���������߸�����Ľ��㣬ȡ���ֵ��
/// </summary>
public class CreeperLegGhostController : ComponentHelperBase<ChainIKConstraint>
        , IAC_CommonSetting_CursorSizeHandler
{
    //ToAdd:���ƿ��ƶ�����Ϊԭ���ָ��Բ�����䣨��TweenMoveAsync���ж��Ƿ�����ƶ���
    public bool NeedMove { get { return isExcessive && !isMoving; } }
    public float CompWeight { get { return Comp.weight; } set { Comp.weight = value; } }
    public float MaxReachDistanceFinal { get { return maxReachDistance * settingCursorSize; } }//���Թ������ֵ
    public float UpdatePositionDistanceFinal { get { return updatePositionDistance * settingCursorSize; } }
    public Transform tfSourceTarget { get { return Comp.data.target; } }//����ʱ��chainIKConstraint�л�ȡ��ע��Ҫ��ģ�ͷֿ��ڷţ����������λ��Ӱ��
    public Transform tfGhostLeg;//�ŵ�Ŀ���
    public float updatePositionDistance = 0.1f;//����Ƿ���Ҫ�ƶ�(������Ŀ���ľ��볬��һ���������½�λ��)
    public float maxReachDistance = 0.3f;//�����ƶ�����Զ����
    //ToAdd�����ƹؽ���ת���򣬱�����ָ�ж�ֻ�����ŵ���������ת
    public Vector2 weightRange = new Vector2(0, 1);
    public float tweenDuration = 0.08f;
    public Ease easeLegUp = Ease.Linear;
    public Ease easeLegDown = Ease.Linear;

    [Header("Runtime")]
    public float curDistance;
    public bool isExcessive = false;//�����������Ҫ�ƶ���
    public bool isMoving = false;//�����ƶ�
    public Vector3 targetPos;

    //Pivot
    CreeperGhostController creeperGhostController;
    public Transform tfModelRoot;//ģ������
    public Vector3 localPivotPos;//���ƶ������ģ���������ɵľֲ�λ�ã�ע����Ϊ����ͬ������˲���Ҫ���Թ��ߴ磩
    private void Awake()
    {
        //Init
        //tfSourceTarget = Comp.data.target;
        tfGhostLeg.position = tfSourceTarget.position = Comp.data.tip.position;//ͬ����ʼλ��

        targetPos = tfGhostLeg.position;
        settingCursorSize = AC_ManagerHolder.CommonSettingManager.CursorSize;

        //�Խ�ĩ�˼����ɵ��е���Ϊ�ŵ�ê��
        creeperGhostController = transform.GetComponentInParent<CreeperGhostController>(true);
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
            if (!isExcessive)//����Ҫ�жϣ�ǿ�Ƹ���λ��
                return;

        isMoving = true;

        ///-����Ŀ�ĵ���ê���������뾶��Χ����Ľ��㣨����������ڣ���ֱ��ʹ��Ŀ�ĵ㣩��Ȼ��ȡ���Ŀ�ĵصĵ�
        /// PS:
        /// -��Ϊ�ų����ޣ������λ��ֻ������Ŀ�����ߵ�ͶӰ������ΪmoveFootDistance��λ��
        Vector3 worldPivotPos = tfModelRoot.TransformPoint(localPivotPos);
        Vector3 vector = tfGhostLeg.position - worldPivotPos;
        float vectorLength = vector.magnitude;

        //Todo:���targetPos�Ƿ��ڿ��ƶ���Χ��
        if ((vectorLength - MaxReachDistanceFinal) < 0)//�ڿ��ƶ�������:ֱ��ʹ��Ŀ��λ��
        {
            targetPos = tfGhostLeg.position;
        }
        else//�ڿ��ƶ������⣺ʹ��������Զ��
        {
            Vector3 vectorNormal = vector.normalized;
            vectorNormal.Scale(Vector3.one * MaxReachDistanceFinal);
            targetPos = worldPivotPos + vectorNormal;
        }

        /// Ų�����������ChainIKConstraint��
        /// -��Weight����Ϊ0
        /// -��Target��λ������Ϊ��ŵ�
        /// -��Weight����Ϊ1������ͨ��Tween��
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

#if UNITY_EDITOR
    #region Editor
    [Header("Editor")]
    public float gizmosRadius = 0.1f;
    public bool gizmosShowDistance = false;
    private void OnDrawGizmos()
    {
        //����Pivot
        if (tfModelRoot)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(tfModelRoot.TransformPoint(localPivotPos), gizmosRadius);
            Gizmos.color = Color.white;
        }

        if (tfGhostLeg)//��ɫ����Ghost
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(tfGhostLeg.position, gizmosRadius);
            Gizmos.color = Color.white;
        }

        Gizmos.DrawWireSphere(targetPos, gizmosRadius);//��ɫ����Ŀ��λ��

        if (Application.isPlaying && tfSourceTarget)//����ɫ�������ӽ���
        {
            float distancePercent = Vector3.Distance(targetPos, tfSourceTarget.position) / MaxReachDistanceFinal;
            Color color = Color.Lerp(Color.green, Color.red, distancePercent);
            Gizmos.color = color;
            if (gizmosShowDistance)
            {
                UnityEditor.Handles.Label(transform.position, $"{(int)(distancePercent * 100)}%");//���Ƶ�ǰ����
            }
            Gizmos.DrawLine(targetPos, tfSourceTarget.position);
        }
    }
    #endregion
#endif
}
