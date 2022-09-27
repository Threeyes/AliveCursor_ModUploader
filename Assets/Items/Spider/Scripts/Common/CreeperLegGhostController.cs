using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System.Threading.Tasks;
/// <summary>
/// ���ܣ�
/// -��עSpider�����ŵ���ŵ㣬ĳ��������ŵ�ľ������ʱ���Ϳ�ʼŲ������
/// 
/// Todo:
/// -Ҳ��Ҫ���浱ǰ�ŵĹ̶��㣬������ʹŲ���������壬��Ҳ�ǹ̶���ԭ��
/// -��Ҫ���ٶ�ӦGhostLegĿ�꣬���ǵ�������ƫ�ƴﵽһ��ֵ���Զ��ƶ��������ܽ��Boredʱ������ת���½�Ť��һ�ŵ�����
/// -Ū�ɾֲ�����ĳ��ȣ����߳���SpiderGhostController�����ţ�
/// </summary>
public class CreeperLegGhostController : ComponentHelperBase<ChainIKConstraint>
        , IAC_CommonSetting_CursorSizeHandler
{
    public bool NeedMove { get { return isExcessive && !isTweening; } }
    public float CompWeight { get { return Comp.weight; } set { Comp.weight = value; } }

    public Transform tfGhostLeg;//�ŵ�Ŀ���
    public float legMaxDistance = 0.1f;//�����ƶ�����Զ����(��������ŵ�ľ��볬��һ���������½�λ��)
    public Vector2 weightRange = new Vector2(0, 1);
    public float tweenDuration = 0.1f;
    public Ease easeLegUp = Ease.Linear;
    public Ease easeLegDown = Ease.Linear;

    [Header("Runtime")]
    public Transform tfSourceTarget;//����ʱ��chainIKConstraint�л�ȡ��ע��Ҫ��ģ�ͷֿ��ڷţ����������λ��Ӱ��
    public float curDistance;
    public bool isExcessive = false;//�������
    public bool isTweening = false;
    public Vector3 targetPos;


    private void Awake()
    {
        tfSourceTarget = Comp.data.target;
        tfGhostLeg.position = tfSourceTarget.position = Comp.data.tip.position;//ͬ����ʼλ�ã�Ϊ�˷���ͬ����chainIKConstraint��Ĭ��Weight��������Ϊ0��

        targetPos = tfGhostLeg.position;
        settingCursorSize = AC_ManagerHolder.CommonSettingManager.CursorSize;
    }
    public float LegMaxDistanceFinal { get { return legMaxDistance * settingCursorSize; } }//���Թ�������ֵ

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
        if (!isExcessive)//����Ҫ�жϣ�ǿ�Ƹ���λ��
            return;

        isTweening = true;
       
        /// Ų�����������ChainIKConstraint��
        /// -��Weight����Ϊ0
        /// -��Target��λ������Ϊ��ŵ�
        /// -��Weight����Ϊ1������ͨ��Tween��
        /// PS:
        /// -��Ϊ�ų����ޣ������λ��ֻ������Ŀ�����ߵ�ͶӰ������ΪmoveFootDistance��λ��
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
