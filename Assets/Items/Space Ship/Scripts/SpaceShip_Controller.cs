using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//To Complete
///散架实现方式：
///1.爆炸时创建克隆体，原物体不变
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
    /// 频繁按下中间键：使用Dummy进行替换，爆炸并波及附近物体，然后出现一段时间无响应的状态(经测试可用)
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator IETestExplosion(float time)
    {
        ////Todo:增加一个onRapidClick的事件，然后做一个连环爆炸，朝向相机方向飞来，并且会散架（子物体detach，各自记录位置，然后加上刚体）
        //_rigidbody.isKinematic = false;
        //_rigidbody.AddExplosionForce(testExplosionPower, _rigidbody.position + UnityEngine.Random.insideUnitSphere, testExplosionRadius);
        //_rigidbody.AddRelativeTorque(testTorquePower * UnityEngine.Random.insideUnitSphere, ForceMode.Force);

        yield return new WaitForSeconds(time);
        //_rigidbody.isKinematic = true;
    }

}
