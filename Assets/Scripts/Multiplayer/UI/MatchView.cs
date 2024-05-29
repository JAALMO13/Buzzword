using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using FishNet;
using FirstGearGames.LobbyAndWorld.Clients;
using FirstGearGames.LobbyAndWorld.Lobbies;
using System.Collections.Generic;
using FishNet.Object;
using DG.Tweening;
public class MatchView : View
{
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI opponentName;
    [SerializeField]
    Image playerAvatar;
    [SerializeField]
    Image opponentAvatar;
    [SerializeField]
    Transform playerUI;
    [SerializeField]
    Transform opponentUI;
    [SerializeField]
    TextMeshProUGUI timerText;
    [SerializeField]
    float waitTime = 5;
    List<NetworkObject> players = new();
    bool start = false;

    [SerializeField]
    Button exitButton;

    [SerializeField]
    CanvasGroup findingCanvas;
    [SerializeField]
    CanvasGroup matchCanvas;
    [SerializeField]
    CanvasGroup countdownCanvas;

    [SerializeField]
    TextMeshProUGUI matchingStatement;
    // # change colour of text for avatar when ready
    bool finished = false;
    int index = 0;
    private float lastColorChangeTime = 0f;
    [SerializeField]
    List<Color32> colours = new();
    [SerializeField]
    float changeTime;

    // public override void OnEnable() {
    //     base.OnEnable();

    // }
    public override void SetupAnimation()
    {
        base.SetupAnimation();
        findingCanvas.alpha = 0;
        opponentName.text = "";
        opponentAvatar.GetComponentInChildren<TextMeshProUGUI>().text = "?";
        matchingStatement.text = "FINDING YOUR MATCH..";
        matchCanvas.alpha = 0;
        countdownCanvas.alpha = 0;
        matchingStatement.alpha = 1;
        for (int i = 0; i < colours.Count; i++)
        {
            int rnd = Random.Range(0, colours.Count);
            Color temp = colours[rnd];
            colours[rnd] = colours[i];
            colours[i] = temp;
        }
    }
    public override void Initialise()
    {
        base.Initialise();
        move = 0.5f;
        exitButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.Show<TitleView>();
            AudioManager.Instance.PlayPressed();
        });
        SetupAnimation();
    }

    public override void Update()
    {
        base.Update();
        if (!isInitialised) return;
        // if(Timer.current == null) return;
        // if (Timer.current.currentTime <= 0) setStartTime();
        if (LobbyNetwork.CurrentRoom != null) players = LobbyNetwork.CurrentRoom.MemberIds;
        if (players.Count == 0) return;
        if (findingCanvas.alpha != 1) return;
        if (!once)
        {
            once = true;
            lastColorChangeTime = Time.time;
            
        }
        if (Time.time - lastColorChangeTime >= changeTime && players.Count == 1)
        {
            index = (index + 1) % colours.Count;
            opponentAvatar.color = colours[index];
            lastColorChangeTime = Time.time;
        }
        if (finished)
        {

            // once = true;
            // opponentAvatar.transform.parent.gameObject.SetActive(true);
            // // if first player in member id start game;
            // for (int i = 0; i < players.Count; i++)
            // {
            //     if (players[0].LocalConnection == InstanceFinder.ClientManager.Clients[players[i].OwnerId]) LobbyNetwork.StartGame();
            //     if (players[i].IsOwner) continue;

            //     PlayerSettings opponentSettings = players[i].GetComponent<PlayerSettings>();
            //     opponentName.text = opponentSettings.GetUsername();
            //     opponentAvatar.color = opponentSettings.GetColour();
            //     opponentAvatar.GetComponentInChildren<TextMeshProUGUI>().text = opponentSettings.GetUsername().ToUpper()[0].ToString();

            // }
            // setStartTime();
            // StartCoroutine(startTimer());
        }

        if (Timer.current != null) timerText.text = ((int)Timer.current.getTime() + 1).ToString();

        if (Timer.current != null && Timer.current.getTime() <= 0f && start) StartCoroutine(ExitAnimation());
    }

    // ! change to allow multiple game modes
    private void changeView()
    {
        ViewManager.Instance.Show<AnagramGameView>();
        GameManager.Instance.StartGame();
    }

    IEnumerator startTimer()
    {
        if (!start && ViewManager.Instance.currentView == this)
        {
            yield return new WaitForSeconds(1f);
            start = true;
            Timer.current.startTimer();
        }
    }
    public void setStartTime()
    {
        start = false;
        StartCoroutine(timerSet());
    }

    IEnumerator timerSet()
    {
        yield return new WaitUntil(() => Timer.current != null);
        Timer.current.currentTime = waitTime - 1f;
    }

    int ClientIndex()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].LocalConnection == InstanceFinder.ClientManager.Clients[players[i].OwnerId]) return i;
        }
        return -1;
    }

    public override IEnumerator Animate()
    {
        findingCanvas.DOFade(1, move);
        yield return new WaitUntil(() => players.Count > 1);
        yield return new WaitForSeconds(0.25f);
        int curI = ClientIndex();
        int oppI = 1 - curI;
        matchingStatement.text = "YOU'RE PLAYING WITH..";
        PlayerSettings opponentSettings = players[oppI].GetComponent<PlayerSettings>();
        opponentName.text = opponentSettings.GetUsername();
        opponentAvatar.color = opponentSettings.GetColour();
        opponentAvatar.GetComponentInChildren<TextMeshProUGUI>().text = opponentSettings.GetUsername().ToUpper()[0].ToString();
        yield return new WaitForSeconds(1.5f);
        playerAvatar.transform.parent.gameObject.SetActive(true);
        PlayerSettings playerSettings = players[curI].GetComponent<PlayerSettings>();
        playerName.text = playerSettings.GetUsername();
        playerAvatar.color = playerSettings.GetColour();
        playerAvatar.GetComponentInChildren<TextMeshProUGUI>().text = playerSettings.GetUsername().ToUpper()[0].ToString();
        matchingStatement.DOFade(0, move / 2);
        matchCanvas.DOFade(1, move);
        opponentUI.DOScale(1, move);
        opponentUI.DOLocalMove(new(-playerUI.localPosition.x, playerUI.localPosition.y), move);
        yield return new WaitUntil(() => !DOTween.IsTweening(opponentUI));
        yield return new WaitForSeconds(1.25f);
        countdownCanvas.DOFade(1, move/2);
        yield return new WaitUntil(() => !DOTween.IsTweening(countdownCanvas));
        if(players[0].IsOwner) LobbyNetwork.StartGame();
        setStartTime();
        StartCoroutine(startTimer());
    }

    IEnumerator ExitAnimation()
    {
        findingCanvas.DOFade(0, move);
        matchCanvas.DOFade(0, move);
        countdownCanvas.DOFade(0, move);
        yield return new WaitUntil(() => !DOTween.IsTweening(countdownCanvas));
        changeView();
    }
}
