using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Threeyes.EventPlayer;
public class Snake_Controller : MonoBehaviour
{
    [Header("Common")]
    //public UnityEvent onEyeBlink = new UnityEvent();//眨眼（PS:因为UMod导出后的事件有问题，所以暂时不使用该方案）
    public List<EventPlayer_SOAction> listEventPlayerSOAction_Eyelid = new List<EventPlayer_SOAction>();
    public Vector2 blinkIntervalDurationRange = new Vector2(3, 5);

    [Header("Bored")]
    public Animator animatorBeak;
    public Light lightBeak;//嘴巴中的灯光
    public Vector2 boredOpenBeakIntervalRange = new Vector2(2, 100);



    [Header("Runtime")]
    public float nextblinkIntervalDuration = 3;
    public float lastBlinkTime = 0;
    private void Update()
    {
        if (Time.time - lastBlinkTime > nextblinkIntervalDuration)
        {
            Blink();
            nextblinkIntervalDuration = Random.Range(blinkIntervalDurationRange.x, blinkIntervalDurationRange.y);
            lastBlinkTime = Time.time;
        }
    }

    void Blink()
    {
        //onEyeBlink.Invoke();
        listEventPlayerSOAction_Eyelid.ForEach(ep => ep.Play());
    }

    #region Invoke by extern (CIB/CSB)
    protected Coroutine cacheEnumAutoOpenBeak;
    public void OnStateEnterExit(bool isEnter)
    {
        TryStopAutoOpenBeakCoroutine();//任意State都会暂停协程
        if (isEnter)
        {
            cacheEnumAutoOpenBeak = StartCoroutine(IEAutoOpenBeak());
        }
        lightBeak.gameObject.SetActive(isEnter);
    }
    protected virtual void TryStopAutoOpenBeakCoroutine()
    {
        if (cacheEnumAutoOpenBeak != null)
            StopCoroutine(cacheEnumAutoOpenBeak);
        animatorBeak.SetBool("Beak Open", false);//闭嘴
    }
    IEnumerator IEAutoOpenBeak()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(boredOpenBeakIntervalRange.x, boredOpenBeakIntervalRange.y));
            animatorBeak.SetBool("Beak Open", true);
            yield return new WaitForSeconds(Random.Range(1, 3));
            animatorBeak.SetBool("Beak Open", false);
        }
    }
    #endregion
}
