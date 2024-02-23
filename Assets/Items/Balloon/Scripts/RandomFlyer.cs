using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Newtonsoft.Json;
using Threeyes.Steamworks;
using Threeyes.Core;

/// <summary>
/// 在场景中随机飞行的物体
/// </summary>
public class RandomFlyer : GenerableObject<RandomFlyer.ConfigInfo>
{
	public Rigidbody rig;
	public MeshRenderer meshRenderer;
	public GameObject preExplodeParticle;
	public UnityEvent onComplete;

	[Header("Run Time")]
	public float curSpeed = 0;
	public float startUniformScale = 1;//Launch前的归一化大小
	public Quaternion newQuaternion;
	public bool isCollisionEnter;
	Vector3 uniqueId;//作为PerlinNoise的y参
	Vector3 basicScale = Vector3.one;//基础缩放（决定气球基础缩放形状)

	protected Coroutine cacheEnum;

	public override void Init(ConfigInfo config)
	{
		base.Init(config);
		uniqueId = Random.insideUnitSphere;
		Color basColor = Random.ColorHSV(0, 1, 0.4f, 0.8f, 0.4f, 0.8f);
		basColor.a = Random.Range(0.7f, 0.9f);
		meshRenderer.material.SetColor("_BaseColor", basColor);

		float diameter = Random.Range(0.9f, 1.1f);//随机气球直径
		basicScale = new Vector3(diameter, diameter, 1);//保证直径一致

		//PS:气球出现时大小要从0 Tween到当前值，避免连续生成导致不连贯
		transform.localScale = Vector3.zero;
	}

	public void SetSize(float uniformScale)
	{
		transform.localScale = GetLocalScale(uniformScale);
		SetStartUniformScale(uniformScale);
	}
	Vector3 GetLocalScale(float uniformScale)
	{
		return/* AC_ManagerHolder.CommonSettingManager.CursorSize * */basicScale * uniformScale;
	}
	public void TweenSetSize(float uniformScale, float tweenDuration = 0.1f, UnityAction actionOnEnlargeComplete = null)
	{
		Vector3 targetLocalScale = GetLocalScale(uniformScale);
		transform.DOScale(targetLocalScale, tweenDuration).SetEase(Ease.OutSine).onComplete += () => actionOnEnlargeComplete.Execute();
		SetStartUniformScale(uniformScale);
	}

	/// <summary>
	/// 设置Launch前的基础缩放
	/// </summary>
	/// <param name="uniformScale"></param>
	void SetStartUniformScale(float uniformScale)
	{
		startUniformScale = uniformScale;
	}

	static Vector3 minSize = Vector3.one * 0.01f;//最小的尺寸
	[ContextMenu("Launch")]
	public void Launch()
	{
		transform.SetParent(null);
		rig.isKinematic = false;
		curSpeed = 0;
		cacheEnum = StartCoroutine(IELaunch());
	}
	IEnumerator IELaunch()
	{
		float startTime = Time.time;
		float lifeTime = startUniformScale * Config.lifeTimePerUnitScale;
		Vector3 startLocalScale = transform.localScale;
		float usedLifePercent = 1;
		newQuaternion = transform.rotation;
		float delayStartRandomChange = Random.Range(Config.delayRandomMovementRange.x, Config.delayRandomMovementRange.y);
		while (true)
		{
			usedLifePercent = (Time.time - startTime) / lifeTime;
			curSpeed = Mathf.Clamp(curSpeed + Config.maxMoveSpeed * Time.deltaTime, 0, Config.maxMoveSpeed);
            curSpeed *= AC_ManagerHolder.CommonSettingManager.CursorSize;
            curSpeed = Mathf.Clamp(curSpeed, 2, 5);//限制初始速度，避免太快或太慢
			if (isCollisionEnter)
			{
				//PS：在碰到刚体时，让物理引擎自行决定旋转值，通常会出现随机颤动
			}
			else
			{
				if (Time.time - startTime > delayStartRandomChange)//持续更改朝向
				{
					////Ref: FlockBox.WanderBehaviour,随机平滑更改朝向
					newQuaternion = Quaternion.Euler(
						Mathf.PerlinNoise(Time.time * Config.wanderIntensity + uniqueId.x, uniqueId.x) * 360,
						Mathf.PerlinNoise(Time.time * Config.wanderIntensity + uniqueId.y, uniqueId.x) * 360,
						Mathf.PerlinNoise(Time.time * Config.wanderIntensity + uniqueId.z, uniqueId.x) * 360);
				}
				rig.rotation = Quaternion.Slerp(rig.rotation, newQuaternion, Config.rotateSpeed * Time.deltaTime);
			}
			rig.velocity = -transform.forward * curSpeed;//保证矢量及速度正确

			usedLifePercent = Mathf.Clamp01(usedLifePercent);
			if (usedLifePercent < 1)
			{
				Vector3 result = Vector3.Lerp(startLocalScale, minSize, usedLifePercent);//ToDo:增加变形效果
				if (result.Avaliable())
					transform.localScale = result;
			}
			else//超时：销毁
			{
				//ToDo:回到Pool
				onComplete.Invoke();
				DestroySelf();
				yield break;
			}
			yield return null;
		}
	}

	public void Explode()
	{
		GameObject goEffect = preExplodeParticle.InstantiatePrefab(null, transform.position, transform.rotation);
		goEffect.transform.localScale = transform.lossyScale;
		DestroySelf();
	}
	private void DestroySelf()
	{
		if (gameObject)
		{
			if (cacheEnum != null)
				StopCoroutine(cacheEnum);
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		isCollisionEnter = true;
	}
	void OnCollisionExit(Collision collision)
	{
		isCollisionEnter = false;
	}

	#region Define
	[System.Serializable]
	[JsonObject(MemberSerialization.Fields)]
	public class ConfigInfo
	{
		[Header("Spawn")]
		public float lifeTimePerUnitScale = 10;//When generated, the total lifetime=localScale*this value
		public Vector2 delayRandomMovementRange = new Vector2(0.1f, 0.3f);//Delay active random movement

		[Header("Motion")]
		public float maxMoveSpeed = 2f;//Max Move Speed
		public float rotateSpeed = 2f;//RotateSpeed
		public float wanderIntensity = 0.8f;//How quickly the wander force can change direction
	}
	#endregion
}
