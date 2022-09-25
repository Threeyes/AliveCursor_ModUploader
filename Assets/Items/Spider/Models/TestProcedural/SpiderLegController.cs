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
/// Ų�����������ChainIKConstraint��
/// -��Weight����Ϊ0
/// -��Target��λ������Ϊ��ŵ�
/// -��Weight����Ϊ1������ͨ��Tween��
/// 
/// 
/// Todo:
/// -Ҳ��Ҫ���浱ǰ�ŵĹ̶��㣬������ʹŲ���������壬��Ҳ�ǹ̶���ԭ��
/// -Ū�ɾֲ�����ĳ��ȣ����߳���SpiderGhostController�����ţ�
/// </summary>
public class SpiderLegController : MonoBehaviour
{
    public ChainIKConstraint chainIKConstraint;
    public Transform tfFoothold;//��ŵ�

    public float moveFootDistance = 0.1f;//��������ŵ�ľ��볬��һ���������½�λ��
    public float tweenDuration = 0.1f;

    [Header("Runtime")]
    public Transform tfSourceTarget;//����ʱ��chainIKConstraint�л�ȡ��ע��Ҫ��ģ�ͷֿ��ڷţ����������λ��Ӱ��
    public float curDistance;

    public bool NeedMove { get { return isExcessive && !isTweening; } }
    public bool isExcessive = false;//�������
    public bool isTweening = false;
    public Vector3 targetPos;
    private void Awake()
    {
        tfSourceTarget = chainIKConstraint.data.target;
        tfFoothold.position = tfSourceTarget.position = chainIKConstraint.data.tip.position;//ͬ����ʼλ�ã�Ϊ�˷���ͬ����chainIKConstraint��Ĭ��Weight��������Ϊ0��

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
        if (!isExcessive)//����Ҫ�жϣ�ǿ�Ƹ���λ��
            return;

        isTweening = true;

        //PS:��Ϊ�ų����ޣ������λ��ֻ������Ŀ�����ߵ�ͶӰ������ΪmoveFootDistance��λ��
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
