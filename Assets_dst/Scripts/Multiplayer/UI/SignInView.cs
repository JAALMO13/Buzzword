using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using FirstGearGames.LobbyAndWorld.Clients;

public class SignInView : View
{
    [SerializeField]
    Image logo;
    [SerializeField]
    TMP_InputField input;
    [SerializeField]
    Button enterButton;
    [SerializeField]
    Image outline;
    [SerializeField]
    Color outlineColour;
    [SerializeField]
    GameObject welcome;
    [SerializeField]
    GameObject signIn;
    [SerializeField]
    GameObject colourSelector;
    [SerializeField]
    GameObject colours;
    [SerializeField]
    Button doneButton;
    [SerializeField]
    Transform selector;
    [SerializeField]
    Color buttonColour;

    float move = 1.5f;
    int selected = -1;
    [SerializeField]
    int minChars = 6;
    [SerializeField]
    View title;
    Color defaultColour;
    int moveDist = 150;

    private void Start()
    {
        colours.SetActive(true);
        selector.gameObject.SetActive(true);
        selector.position = Vector3.one * - 1000;
        logo.gameObject.SetActive(true);
        logo.transform.position = new Vector3(Screen.width / 2, Screen.height / 8 -20);
        input.transform.parent.GetComponent<CanvasGroup>().alpha = 0;
        welcome.GetComponent<TextMeshProUGUI>().alpha = 0;
        welcome.transform.DOMoveY(welcome.transform.position.y - moveDist, 0);
        input.transform.parent.DOMoveY(input.transform.parent.position.y - moveDist, 0);
        defaultColour = outline.color;
    }

    public override void Initialise()
    {
        base.Initialise();
        doneButton.interactable = false;
        StartCoroutine(Animate());
        input.onSelect.AddListener(inp => ChangeOutline(outlineColour));
        input.onDeselect.AddListener(inp => ChangeOutline(defaultColour));
        enterButton.onClick.AddListener(() =>
        {
            // save username to player prefs or some way to save details
            PlayerSettingsHolder.Instance.username = input.text;
            PlayerSettingsHolder.Instance.changed = true;
            signIn.SetActive(false);
            colourSelector.SetActive(true);
        });

        foreach(Transform child in colours.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(()=> GetSelected(child));
        }
        doneButton.interactable = false;
        doneButton.onClick.AddListener(() =>
        {
            PlayerSettingsHolder.Instance.colour = colours.transform.GetChild(selected).GetComponent<Image>().color;
            Hide();
            title.Show();
            title.Initialise();
        });
    }

    private void Update()
    {
        if (!isInitialised) return;
        if (!string.IsNullOrEmpty(input.text)) ChangeOutline(outlineColour);
        if(input.text.Length >= minChars){
            enterButton.transform.parent.GetComponent<Image>().color = buttonColour;
            enterButton.transform.GetChild(0).GetComponent<Image>().color = buttonColour;
            enterButton.interactable = true;
        }
        else{
            enterButton.interactable = false;
        }
        if (!once && colourSelector.activeSelf)
        {
            once = true;
            StartCoroutine(Animate());
        }
        if (selected >= 0)
        {
            doneButton.interactable = true;
            doneButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            selector.gameObject.SetActive(true);
            selector.position = colours.transform.GetChild(selected).position;
        }


    }

    public override void ResetView()
    {
        base.ResetView();
        input.onSelect.RemoveAllListeners();
    }

    private void ChangeOutline(Color colour)
    {
        outline.color = colour;
    }

    IEnumerator Animate()
    {
        if (!colourSelector.activeSelf)
        {
            welcome.transform.DOMoveY(welcome.transform.position.y + moveDist, move);
            welcome.GetComponent<TextMeshProUGUI>().DOFade(1, move);
            yield return new WaitUntil(() => !DOTween.IsTweening(welcome.transform));
            input.transform.parent.GetComponent<CanvasGroup>().DOFade(1, move);
            input.transform.parent.DOMoveY(input.transform.parent.position.y + moveDist, move);
        }
        else{

        }
    }

    void GetSelected(Transform child){
        selected = child.GetSiblingIndex();
    }
}
