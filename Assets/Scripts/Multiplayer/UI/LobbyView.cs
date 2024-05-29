using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyView : View
{

    [SerializeField]
    private TMP_Dropdown dropdown;

    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private Button joinButton;

    [SerializeField]
    private Button searchButton;

    [SerializeField]
    private Button hostButton;


    public override void Initialise()
    {
        base.Initialise();
    }

    public override void Update()
    {
        base.Update();
    }
}
