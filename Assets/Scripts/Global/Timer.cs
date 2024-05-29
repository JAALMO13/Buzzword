using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] bool isCountDown = true;
    [SerializeField] bool play = true;
    public float currentTime = 0f;//Serialized for debugging makes it easier to see the time

    public static Timer current;
    public float startTime;


    private void Awake()
    {
        current = this;
        currentTime = startTime;
    }

    void Update()
    {
        if (play)
        {
            if (isCountDown)
            {
                // decrease time when play is active
                currentTime -= Time.deltaTime;

                if (currentTime <= 0)
                {
                    currentTime = 0;
                    play = false;// stop timer
                    Timer.current.TimerFinish();
                }
            }
            else
            {
                // increase time when play is active
                currentTime += Time.deltaTime;
            }
        }
    }

    //Create event callback to check when timer is finished
    public event Action onTimerFinish;
    public void TimerFinish()
    {
        if(onTimerFinish != null)
        {
            onTimerFinish();
        }
    }

    public void startTimer()
    {
        play = true;
    }

    public void pauseTimer()
    {
        play = false;
    }

    public float getTime()
    {
        return currentTime;
    }

    public float getTimeRounded2DP()
    {
        double doubleTime = (Mathf.Round(currentTime * 100)) / 100.0;

        return (float)doubleTime;
    }

    //gives class the ability to add to timer
    public void AddTime(float additionalTime)
    {
        currentTime += additionalTime;
    }

    //gives class the ability to delay timer
    public void RemoveTime(float additionalTime)
    {
        currentTime -= additionalTime;
    }

    public void SetTime(float timetoSetTo)
    {
        currentTime = timetoSetTo;
    }

    public void SetStartTime(float startTimeToSetTo)
    {
        startTime = startTimeToSetTo;
    }

    public string ColonTime(float timeInSeconds){
        return (timeInSeconds / 60).ToString() + ":" + (timeInSeconds % 60).ToString();
    }

    public void SetIsCountdown(bool val){
        isCountDown = val;
    }
}
