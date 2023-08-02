using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using Threeyes.Steamworks;
/// <summary>
/// Todo：改为通用的生成器，放到SDK中
/// </summary>
public class BalloonGenerator : ObjectGenerator
{
	public BoolEvent onSimulateKeyClick;//Simulate the target key get pressed

	//Extra Config(PS：这些设置并不通用，不能放到基类的ConfigInfo中）
	[Header("Bored")]
	[Tooltip("The interval time range that generate new obj on specify state")]
	public Vector2 boredGenerateIntervalRange = new Vector2(1, 5);
	public Vector2 boredGenerateSizeRange = new Vector2(0.1f, 1f);

	public RandomFlyer.ConfigInfo elementConfig;

	//Runtime
	RandomFlyer curRandomFlyer;
	protected Coroutine cacheEnumAutoGenerate;

	protected void OnDisable()//PS:隐藏或销毁都会调用
	{
		TryStopAutoGenerateCoroutine();
		if (curRandomFlyer)
		{
			curRandomFlyer.Explode();
			curRandomFlyer = null;
		}
	}
	protected virtual void TryStopAutoGenerateCoroutine()
	{
		if (cacheEnumAutoGenerate != null)
			StopCoroutine(cacheEnumAutoGenerate);
	}
	IEnumerator IEAutoRandomGenerate()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(boredGenerateIntervalRange.x, boredGenerateIntervalRange.y));

			onSimulateKeyClick.Invoke(true);//Simulate key get pressed down, 模拟按压效果
			var tempFlyer = InitElement();
			tempFlyer.TweenSetSize(Random.Range(boredGenerateSizeRange.x, boredGenerateSizeRange.y), 0.2f,
				() =>
				{
					tempFlyer.Launch();
					onSimulateKeyClick.Invoke(false);//Simulate the key get press up
				});
		}
	}
	protected RandomFlyer InitElement()
	{
		var curRandomFlyer = GetPrefab()?.InstantiatePrefab<RandomFlyer>(transform);
		curRandomFlyer?.Init(elementConfig);//基于局部设置缩放等值，因此不需要考虑预制物大小
		return curRandomFlyer;
	}

	#region Invoke by extern (CIB/CSB)
	public void OnStateEnterExit(bool isEnter)
	{
		TryStopAutoGenerateCoroutine();//任意State都会暂停协程
		if (isEnter)
		{
			cacheEnumAutoGenerate = StartCoroutine(IEAutoRandomGenerate());
		}
	}

	float curReceiverValue;
	public void OnReceiverValueChange(float value)
	{
		curReceiverValue = value;
		//监听Tween回调，持续更改当前气球的大小
		if (curRandomFlyer)
		{
			curRandomFlyer.SetSize(value);
		}
	}

	bool isCurMouseDown = false;
	/// <summary>
	/// (PS: Use CursorInputListener to support custom key)
	/// </summary>
	/// <param name="isDown"></param>
	public void OnMouseDown(bool isDown)
	{
		//#整体交互：
		//点击的时候就会充气，充满会爆炸
		//仅在光标外形为箭头时生成气球，在用户点击链接或编辑文字不会出现，避免干扰
		if (isDown && AC_ManagerHolder.SystemCursorManager.CurSystemCursorAppearanceType == AC_SystemCursorAppearanceType.Arrow)
		{
			if (!curRandomFlyer)
			{
				curRandomFlyer = InitElement();
			}
			else
			{
				if (curReceiverValue >= 1)//当前物体大小大于1则爆炸 (PS:因为松手会值会消退，所以需要在Inspector中可以看到值不是大于1后立即爆炸，而是超过1后需要多次重按）
				{
					curRandomFlyer.Explode();
					curRandomFlyer = null;
				}
			}
		}

		//Todo:如果是快速滚动中键，那就生成长条气球
		isCurMouseDown = isDown;
	}
	public void OnMouseMove()
	{
		//检测光标释放按键时系统光标是否移动，如果移动，则释放气球
		if (curRandomFlyer)
		{
			if (isCurMouseDown == false)
			{
				curRandomFlyer.Launch();
				curRandomFlyer = null;
				//PS:释放后不重置大小，这样方便连续创建类似尺寸的气球
			}
		}
	}
	#endregion
}
