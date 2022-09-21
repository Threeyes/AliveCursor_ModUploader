using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Snake_Controller : MonoBehaviour
{
    public UnityEvent onEyeBlink = new UnityEvent();//еЃбл
    public Vector2 blinkIntervalDurationRange = new Vector2(3, 5);

    [Header("Runtime")]
    public float nextblinkIntervalDuration = 3;
    public float lastBlinkTime = 0;
    private void Update()
    {
        if (Time.time - lastBlinkTime > nextblinkIntervalDuration)
        {
            onEyeBlink.Invoke();
            nextblinkIntervalDuration = Random.Range(blinkIntervalDurationRange.x, blinkIntervalDurationRange.y);
            lastBlinkTime = Time.time;
        }
    }

    void Blink()
    {

    }
}
