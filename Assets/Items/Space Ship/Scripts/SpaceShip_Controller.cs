using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//To Complete
///ɢ��ʵ�ַ�ʽ��
///1.��ըʱ������¡�壬ԭ���岻��
public class SpaceShip_Controller : MonoBehaviour
{

    public float testExplosionTime = 3.0F;
    public float testExplosionPower = 200f;
    public float testTorquePower = 200f;
    public float testExplosionRadius = 5.0F;

    public List<GameObject> listGOBodyPart = new List<GameObject>();

    public void TestExplosion()
    {
        StartCoroutine(IETestExplosion(5));
    }
    /// <summary>
    /// Ƶ�������м����ʹ��Dummy�����滻����ը�������������壬Ȼ�����һ��ʱ������Ӧ��״̬(�����Կ���)
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator IETestExplosion(float time)
    {
        ////Todo:����һ��onRapidClick���¼���Ȼ����һ��������ը���������������������һ�ɢ�ܣ�������detach�����Լ�¼λ�ã�Ȼ����ϸ��壩
        //_rigidbody.isKinematic = false;
        //_rigidbody.AddExplosionForce(testExplosionPower, _rigidbody.position + UnityEngine.Random.insideUnitSphere, testExplosionRadius);
        //_rigidbody.AddRelativeTorque(testTorquePower * UnityEngine.Random.insideUnitSphere, ForceMode.Force);

        yield return new WaitForSeconds(time);
        //_rigidbody.isKinematic = true;
    }

}
