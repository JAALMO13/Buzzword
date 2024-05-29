using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using FirstGearGames.LobbyAndWorld.Clients;
using FishNet.Object;

public class SignInView : View
{
    [SerializeField]
    Image logo;
    [SerializeField]
    TMP_InputField input;
    [SerializeField]
    Button enterButton;
    [SerializeField]
    Outline outline;
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
    [SerializeField]
    Transform chooseText;
    [SerializeField]
    CanvasGroup group;
    int selected = -1;
    [SerializeField]
    int minChars = 6;
    [SerializeField]
    View title;
    [SerializeField]
    int moveStart = 150;
    PlayerSettings player;


    public override void SetupAnimation()
    {
        selector.gameObject.SetActive(true);
        selector.position = Vector3.one * -1000;
        logo.gameObject.SetActive(true);
        logo.transform.position = new Vector3(Screen.width / 2, Screen.height / 8 - 20);
        input.transform.parent.GetComponent<CanvasGroup>().alpha = 0;
        welcome.GetComponent<TextMeshProUGUI>().alpha = 0;
        welcome.transform.DOMoveY(welcome.transform.position.y - moveStart, move);
        input.transform.parent.DOMoveY(input.transform.parent.position.y - moveStart, move);
        chooseText.GetComponent<TextMeshProUGUI>().alpha = 0;
        chooseText.transform.DOMoveY(chooseText.transform.position.y - moveStart, move);
        group.alpha = 0;
    }

    public override void Initialise()
    {
        base.Initialise();
        player = GetPlayer();
        colours.SetActive(true);
        SetupAnimation();
        doneButton.interactable = false;
        input.onSelect.AddListener(inp =>
        {
            ChangeOutline(new Color32(43, 43, 43, 255));
            AudioManager.Instance.PlayPressed();
        });
        input.onDeselect.AddListener(inp => ChangeOutline(new Color32(225, 225, 225, 255)));
        enterButton.onClick.AddListener(() =>
        {
            // save username to player prefs or some way to save details
            // PlayerSettingsHolder.Instance.username = input.text;
            player.SetUsername(input.text);
            signIn.SetActive(false);
            colourSelector.SetActive(true);
            AudioManager.Instance.PlayPressed();
        });

        foreach (Transform child in colours.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                GetSelected(child);
                AudioManager.Instance.PlayPressed();
            });
        }
        doneButton.interactable = false;
        doneButton.onClick.AddListener(() =>
        {
            // PlayerSettingsHolder.Instance.colour = colours.transform.GetChild(selected).GetComponent<Image>().color;
            player.SetColour(colours.transform.GetChild(selected).GetComponent<Image>().color);
            PlayerData data = new(player, ColourMode.Instance.colour, AudioManager.Instance);
            SaveSystem.SaveToJson(data);
            ViewManager.Instance.Show<TitleView>();
            AudioManager.Instance.PlayPressed();
        });
    }

    public override void Update()
    {
        base.Update();
        if (!isInitialised) return;
        if (!string.IsNullOrEmpty(input.text)) ChangeOutline(new Color(43, 43, 43));
        if (input.text.Length >= minChars)
        {
            enterButton.GetComponent<Outline>().effectColor = buttonColour;
            enterButton.transform.GetChild(0).GetComponent<Image>().color = buttonColour;
            enterButton.interactable = true;
        }
        else
        {
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

    private void ChangeOutline(Color32 colour)
    {
        outline.effectColor = colour;
    }

    public override IEnumerator Animate()
    {
        if (!colourSelector.activeSelf)
        {
            welcome.transform.DOMoveY(welcome.transform.position.y + moveStart, move);
            welcome.GetComponent<TextMeshProUGUI>().DOFade(1, move);
            yield return new WaitUntil(() => !DOTween.IsTweening(welcome.transform));
            input.transform.parent.GetComponent<CanvasGroup>().DOFade(1, move);
            input.transform.parent.DOMoveY(input.transform.parent.position.y + moveStart, move);
        }
        else
        {
            welcome.GetComponent<TextMeshProUGUI>().DOFade(0, move / 2);
            input.transform.parent.GetComponent<CanvasGroup>().DOFade(0, move / 2);
            yield return new WaitUntil(() => !DOTween.IsTweening(welcome.transform));
            chooseText.transform.DOMoveY(chooseText.transform.position.y + moveStart, move);
            chooseText.GetComponent<TextMeshProUGUI>().DOFade(1, move);
            yield return new WaitUntil(() => !DOTween.IsTweening(chooseText.transform));
            group.DOFade(1, move);
        }
    }
    
    IEnumerator ExitAnimation<TView>() where TView : View
    {
        group.DOFade(0, move / 2);
        chooseText.GetComponent<TextMeshProUGUI>().DOFade(0, move / 2);
        yield return new WaitUntil(() => !DOTween.IsTweening(group));
        ViewManager.Instance.Show<TView>();
    }

    void GetSelected(Transform child)
    {
        selected = child.GetSiblingIndex();
    }

    PlayerSettings GetPlayer()
    {
        List<GameObject> players = ObjectFinder.Instance.GetPlayers();
        // compare players[i] with client
        foreach (var p in players)
        {
            if (p.GetComponent<NetworkObject>().IsOwner) return p.GetComponent<PlayerSettings>();
        }
        return null;
    }
}
