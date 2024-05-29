using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    [SerializeField]
    private View[] views;

    [SerializeField]
    private View defaultView;
    public View currentView { get; private set; }
    public View previousView { get; private set; }
    public View nextView { get; private set;}
    public LoadingView loadingView;
    [SerializeField]
    private bool autoInitialise;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        if (autoInitialise) Initialise();
        DontDestroyOnLoad(this.gameObject);
    }

    public void Initialise()
    {
        foreach (View view in views)
        {
            view.TryGetComponent<LoadingView>(out loadingView);
            view.Initialise();
            view.Hide();
        }

        if (defaultView != null) Show(defaultView);
    }

    public void ResetViews(){
        foreach(View view in views){
            view.ResetView();
        }
    }
    

    public void Show<TView>(object args = null) where TView : View
    {
        foreach (View view in views)
        {
            if (view is not TView) continue;
            if (currentView != view && currentView != null) currentView.Hide();
            // CheckView<TView>();
            view.Show(args);
            previousView = currentView;
            currentView = view;
        }
    }

    public void Show(View view, object args = null)
    {
        if (currentView != null) currentView.Hide();

        CheckView(view);
        view.Show(args);
        previousView = currentView;
        currentView = view;
    }
    public void Show(string v, object args = null)
    {
        if (currentView != null) currentView.Hide();

        // find gameobject holding the view 
        View view = ObjectFinder.Instance.FindInAll(v).GetComponent<View>();
        CheckView(view);
        view.Show(args);
        previousView = currentView;
        currentView = view;
    }

    public void ShowPrevious()
    {
        if (currentView != null) currentView.Hide();

        previousView.Show();
        View temp = previousView;
        previousView = currentView;
        currentView = temp;
    }

    public void ShowNext(View view, float waitTime, object args = null)
    {
        if (currentView != null) currentView.Hide();
        previousView = currentView;
        Show<LoadingView>();
        // wait time 
        StartCoroutine(Next(view, waitTime));
    }

    public void ShowNext<TView>(float waitTime, object args = null) where TView : View
    {
        if (currentView != loadingView) Show<LoadingView>();
        // wait time 
        View v = null;
        foreach (View view in views)
        {
            if (view is not TView) continue;
            previousView = currentView;
            v = view;
        }
        StartCoroutine(Next(v, waitTime));
    }

    public IEnumerator Next(View view, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (currentView != view && currentView != null) currentView.Hide();
        CheckView(view);
        view.Show();
        currentView = view;
    }

    private void CheckView(View view)
    {
        // flag to track if the new string is already in the array
        bool isInArray = false;

        // iterate through the array
        for (int i = 0; i < views.Length; i++)
        {
            // check if the new string is equal to the current element
            if (views[i] == view)
            {
                // if it is, set the flag and break out of the loop
                isInArray = true;
                break;
            }
        }

        // if the new string isn't in the array, add it
        if (!isInArray)
        {
            // create a new array with room for the new string
            View[] newArray = new View[views.Length + 1];

            // copy the old array into the new array
            for (int i = 0; i < views.Length; i++)
            {
                newArray[i] = views[i];
            }

            // add the new string to the end of the new array
            newArray[newArray.Length - 1] = view;

            // replace the old array with the new array
            views = newArray;
        }
    }

    private void CheckView<TView>() where TView : View
    {
        TView t = default(TView);
        print(t);
        // flag to track if the new string is already in the array
        bool isInArray = false;

        // iterate through the array
        for (int i = 0; i < views.Length; i++)
        {
            // check if the new string is equal to the current element
            if (views[i] is TView)
            {
                // if it is, set the flag and break out of the loop
                isInArray = true;
                break;
            }
        }

        // if the new string isn't in the array, add it
        if (!isInArray)
        {
            // create a new array with room for the new string
            View[] newArray = new View[views.Length + 1];

            // copy the old array into the new array
            for (int i = 0; i < views.Length; i++)
            {
                newArray[i] = views[i];
            }

            // add the new string to the end of the new array
            newArray[newArray.Length - 1] = ObjectFinder.Instance.FindInAll(t.ToString()).GetComponent<TView>();

            // replace the old array with the new array
            views = newArray;
        }
    }

    public void SetNextView(View next){
        nextView = next;
    }
    public void SetNextView<TView>(){
        foreach (View view in views)
        {
            if (view is not TView) continue;
            nextView = view;
        }
    }

}
