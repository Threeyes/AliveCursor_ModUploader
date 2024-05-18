using System.Collections;
using System.Collections.Generic;
using Threeyes.Core;
using UnityEngine;
public class RocketShip_MeteorController : MonoBehaviour
{
    public Rigidbody rig;
    public Renderer rend;
    public void Init(float lifeTime, Vector2 meteorSizeRange, Vector2 velocityPowerRange)
    {
        float cursorSize = AC_ManagerHolder.CommonSettingManager.CursorSize;
        float modelScale = Random.Range(meteorSizeRange.x, meteorSizeRange.y) * cursorSize;

        transform.localPosition = Random.onUnitSphere * Random.Range(10, 20);//在原点附近生成
        transform.localScale = Vector3.one * modelScale;

        rend.material.color = Color.HSVToRGB(0, 0, Random.Range(0, 0.8f));
        rig.mass *= modelScale;

        //朝AC附近发射
        Vector3 direction = (AC_AliveCursor.Instance.transform.position + Random.insideUnitSphere * Random.Range(0, 1) * cursorSize) - transform.position;
        rig.AddForce(direction.normalized * velocityPowerRange, ForceMode.VelocityChange);//Ignore mass

        CoroutineManager.StartCoroutineEx(IEDelayDestroy(lifeTime));
    }

    IEnumerator IEDelayDestroy(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);

    }
}
