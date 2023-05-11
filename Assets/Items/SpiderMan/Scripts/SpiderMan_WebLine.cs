using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 功能：
/// -更新LineRenderer
/// -延迟销毁
/// </summary>
public class SpiderMan_WebLine : AC_GenerableObject<SpiderMan_WebLine.ConfigInfo>
{
	public LineRenderer lineRenderer;
	public Transform tfRoot;
	public Rigidbody rigEnd;
	
	[Header("Runtime")]
	public bool isGrabing = false;
	public override void Init(ConfigInfo config)
	{
		base.Init(config);

		//Todo:Root物体朝Shooter的前方发射（可以是一瞬间；非必要，或者去掉这个过程，直接到达终点）
		transform.eulerAngles = config.tfWebShooter.eulerAngles;//同步发射器的角度
		rigEnd.isKinematic = true;
		isGrabing = true;
		Invoke(nameof(DestroySelf), config.lifeTime);

		StartCoroutine(IEDelayShow());
	}

	IEnumerator IEDelayShow()
	{
		yield return new WaitForEndOfFrame();
		///PS:
		///-因为该物体被创建后默认在原点，因此要延后激活
		lineRenderer.enabled = true;
	}
	public void Release()
	{
		rigEnd.isKinematic = false;
		isGrabing = false;
	}

	private void LateUpdate()
	{
		if (config.tfWebShooter && isGrabing)
			rigEnd.position = config.tfWebShooter.position;//跟随目标位置

		lineRenderer.SetPosition(0, tfRoot.position);
		lineRenderer.SetPosition(1, rigEnd.position);
	}
	private void DestroySelf()
	{
		if (gameObject)
		{
			Destroy(gameObject);
		}
	}
	[System.Serializable]
	[JsonObject(MemberSerialization.Fields)]
	public class ConfigInfo
	{
		public float lifeTime = 2;
		public Transform tfWebShooter;
	}
}
