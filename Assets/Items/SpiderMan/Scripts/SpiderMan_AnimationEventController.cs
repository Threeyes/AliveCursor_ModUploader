using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 功能：
/// -如果用户
/// </summary>
public class SpiderMan_AnimationEventController : MonoBehaviour
{
	public AC_ObjectMovement_FollowTarget objectMovement;
	public Transform tfWebShooter;//Z is the forward direction
	public Transform tfWebLineGroup;//Show/Hide all weblines
	public GameObject goPreWeb;

	[Header("Config")]
	public float webLineLifeTime = 2;

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
			lifeTime= webLineLifeTime
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
}
