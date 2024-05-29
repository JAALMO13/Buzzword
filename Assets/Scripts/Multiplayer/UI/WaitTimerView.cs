using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WaitTimerView : View
{

    // change the back ground image when timer changes
    [SerializeField] TextMeshProUGUI countdown;
    [SerializeField] float waitTime = 5;
    bool start = false;
    public override void Initialise()
    {
        setStartTime();
    }
    public override void Update()
    {
        base.Update();
        StartCoroutine(startTimer());
        countdown.text = ((int)Timer.current.getTime()).ToString();
        if (Timer.current.getTime() <= 1) changeView();
    }

    // ! change to allow multiple game modes
    private void changeView()
    {
        ViewManager.Instance.Show<AnagramGameView>();
        // ViewManager.Instance.Show(view);
    }

    IEnumerator startTimer()
    {
        if (!start && ViewManager.Instance.currentView.name == "WaitTimerView")
        {
            yield return new WaitForSeconds(1f);
            start = true;
            Timer.current.startTimer();
        }
    }
    public void setStartTime()
    {
        start = false;
        Timer.current.currentTime = waitTime;
    }
}

