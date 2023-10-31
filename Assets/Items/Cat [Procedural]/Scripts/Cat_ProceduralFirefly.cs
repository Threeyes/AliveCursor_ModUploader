using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat_ProceduralFirefly : Cat_ProceduralInterestPoint
{
    #region ICat_ProceduralInterestPoint
    public override Vector3 EndPointPos
    {
        get
        {
            //投影到Z平面的点
            return new Vector3(transform.position.x, transform.position.y, 0);
            //return Vector3.ProjectOnPlane(Vector3.back, transform.position);
        }
    }

    public override Vector3 LookTargetPos
    {
        get
        {
            return transform.position;
        }
    }
    #endregion

    public Vector2 delayDisappearTimeRange = new Vector2(3, 5);//Delay active random movement
    public Vector2 moveSpeedRange = new Vector2(2, 4);
    public Vector2 sizeRange = new Vector2(0.5f, 1.5f);
    public float wanderIntensity = 0.3f;
    public float rotateSpeed = 1;

    [Header("Runtime")]
    public float disappearTime;
    public float speed;
    public float lastChangeRotationTime;
    public Vector3 targetLocalEulerAngles;
    private void OnEnable()
    {
        float cursorSize = AC_ManagerHolder.CommonSettingManager.CursorSize;

        disappearTime = Random.Range(delayDisappearTimeRange.x, delayDisappearTimeRange.y);
        speed = Random.Range(moveSpeedRange.x, moveSpeedRange.y) * cursorSize;
        transform.localEulerAngles = GetRandomRot();//随机初始位置
        targetLocalEulerAngles = transform.localEulerAngles;

        transform.localScale = Vector3.one * Random.Range(sizeRange.x, sizeRange.y) * cursorSize;
        StartCoroutine(IEDesoroySelf());
    }

    void Update()
    {
        if (Time.unscaledTime - lastChangeRotationTime > wanderIntensity)
        {
            targetLocalEulerAngles = GetRandomRot();
            lastChangeRotationTime = Time.unscaledTime;
        }
        transform.Translate(Vector3.forward * speed * Time.unscaledDeltaTime, Space.Self);
        transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles, targetLocalEulerAngles, rotateSpeed * Time.unscaledDeltaTime);
    }

    private static Vector3 GetRandomRot()
    {
        return Random.insideUnitSphere * 360;
    }

    IEnumerator IEDesoroySelf()
    {
        yield return new WaitForSecondsRealtime(disappearTime);
        if (gameObject)
            Destroy(gameObject);
    }
}
