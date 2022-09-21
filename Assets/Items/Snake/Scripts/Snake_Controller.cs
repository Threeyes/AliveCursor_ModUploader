using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Threeyes.EventPlayer;
public class Snake_Controller : MonoBehaviour
{
    //public UnityEvent onEyeBlink = new UnityEvent();//眨眼（PS:因为UMod导出后的事件有问题，所以暂时不使用该方案）
    public List<EventPlayer_SOAction> listEventPlayerSOAction = new List<EventPlayer_SOAction>();
    public Vector2 blinkIntervalDurationRange = new Vector2(3, 5);

    [Header("Runtime")]
    public float nextblinkIntervalDuration = 3;
    public float lastBlinkTime = 0;
    private void Update()
    {
        if (Time.time - lastBlinkTime > nextblinkIntervalDuration)
        {
            //onEyeBlink.Invoke();
            listEventPlayerSOAction.ForEach(ep => ep.Play());
            nextblinkIntervalDuration = Random.Range(blinkIntervalDurationRange.x, blinkIntervalDurationRange.y);
            lastBlinkTime = Time.time;
        }
    }

    void Blink()
    {

    }
}
