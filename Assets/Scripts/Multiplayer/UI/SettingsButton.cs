using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsButton : View
{
    public override void Initialise()
    {
        GetComponent<Button>().onClick.AddListener(() => ViewManager.Instance.Show<SettingsView>()); // show settings view
    }

    public override void Update()
    {
        base.Update();
        if (ViewManager.Instance.currentView.name == "SettingsView" || ViewManager.Instance.currentView.name == "MatchView")
        {
            GetComponent<Image>().enabled = false;
            GetComponent<Button>().enabled = false;
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
        }
        else
        {
            GetComponent<Image>().enabled = true;
            GetComponent<Button>().enabled = true;
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = true;
        }
    }
}
