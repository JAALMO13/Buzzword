using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class View : MonoBehaviour
{
    public bool isInitialised { get; private set; }
    public Image[] changeColour;
    public TextMeshProUGUI[] changeTextColour;
    public float move = 1;
    Color oldColour;
    [HideInInspector] public bool once = false;
    public virtual void SetupAnimation() { }
    public virtual void OnEnable()
    {
        StartCoroutine(Animate());
        ResetView();
    }
    public virtual void Initialise()
    {
        isInitialised = true;
    }

    public virtual void Show(object args = null)
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {

        gameObject.SetActive(false);
    }
    public virtual void ResetView()
    {
        once = false;
    }
    private void OnDisable()
    {
        SetupAnimation();
    }

    public virtual void Update()
    {
        if (oldColour != ColourMode.Instance.colour)
        {
            oldColour = ColourMode.Instance.colour;
            foreach (Image i in changeColour)
            {
                i.color = ColourMode.Instance.colour;
            }
            foreach (TextMeshProUGUI t in changeTextColour)
            {
                t.color = ColourMode.Instance.colour;
            }
        }
    }
    public virtual IEnumerator Animate()
    {
        yield return null;
    }
}
