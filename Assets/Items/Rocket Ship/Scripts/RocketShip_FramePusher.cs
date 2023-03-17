using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���ܣ�Ϊ�������Trigger��������������
/// </summary>
public class RocketShip_FramePusher : MonoBehaviour
{
    public float forcePower = 5;

    public void OnTriggerStay(Collider other)
    {
        Rigidbody rigOther = other.attachedRigidbody;
        if (rigOther == null)
            return;
        if (rigOther.isKinematic)
            return;

        Vector3 directionNormalized = transform.TransformDirection(Vector3.back).normalized;
        rigOther.AddForce(directionNormalized * forcePower, ForceMode.Force);
        rigOther.AddTorque(directionNormalized * forcePower, ForceMode.Force);
    }
}
