using System.Collections;
using System.Collections.Generic;
using Threeyes.Action;
using UnityEngine;
/// <summary>
/// Todo：
/// -耳朵会随机扇动，模拟驱赶虫子的效果
/// </summary>
public class Cat_ProceduralEarController : MonoBehaviour
{
	//实现：Animation
	public List<EventPlayer_SOAction> listEventPlayerSOAction_ShakeEar = new List<EventPlayer_SOAction>();
	public Vector2 shakeIntervalDurationRange = new Vector2(20, 60);

	[Header("Runtime")]
	public float nextShakeIntervalDuration = 3;
	public float lastShakeTime = 0;
	void Update()
	{
		if (Time.time - lastShakeTime > nextShakeIntervalDuration)
		{
			Shake();
			nextShakeIntervalDuration = Random.Range(shakeIntervalDurationRange.x, shakeIntervalDurationRange.y);
			lastShakeTime = Time.time;
		}
	}
	[ContextMenu("Shake")]
	public void Shake()
	{
		listEventPlayerSOAction_ShakeEar.ForEach(ep => ep.Play());
	}
}
