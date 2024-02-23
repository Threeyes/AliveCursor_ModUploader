using System.Collections;
using System.Collections.Generic;
using Threeyes.Core;
using UnityEngine;

public class Cat_ProceduralTailController : MonoBehaviour
{
	[Header("Config")]
	public Transform tfTailJoint;
	public Vector3 tailRotateRange = new Vector3(60, 0, 60);
	public float rotateSpeed = 1;
	public float rotateThreshold = 1;
	[Header("Runtime")]
	public Quaternion initLocalRotation;
	[Range(0, 1)] public float rotateChance = 0.95f;
	public Quaternion targetLocalRotation;

	private void Start()
	{
		initLocalRotation = tfTailJoint.localRotation;
		CreateRandomAngle();
	}
	private void Update()
	{
		//ToAdd:身体(AC_CreeperTransformController)移动时不摆动，或者将targetLocalRotation设置为默认旋转值，避免偏移

		tfTailJoint.localRotation = Quaternion.Lerp(tfTailJoint.localRotation, targetLocalRotation, Time.deltaTime * rotateSpeed);
		float angleBetween = Quaternion.Angle(tfTailJoint.localRotation, targetLocalRotation);

		if (angleBetween < rotateThreshold)
		{
			//Try change to new angle
			if (Random.value < rotateChance)
			{
				CreateRandomAngle();
			}
		}
	}

	void CreateRandomAngle()
	{
		targetLocalRotation = Quaternion.Euler(initLocalRotation.eulerAngles + tailRotateRange.RandomRange());
	}

}
