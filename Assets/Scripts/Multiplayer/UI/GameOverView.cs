using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet;
using TMPro;
using FirstGearGames.LobbyAndWorld.Clients;
using FishNet.Object;
using DG.Tweening;
using FirstGearGames.LobbyAndWorld.Lobbies;
using System.Linq;
public class GameOverView : View
{
    [SerializeField]
    TMP_FontAsset font;
    [SerializeField]
    Image background;
    [SerializeField]
    TextMeshProUGUI statePrompt;
    [SerializeField]
    private Button newGameButton;
    [SerializeField]
    private Button rematchButton;

    [SerializeField]
    GameObject playerAvatar;

    [SerializeField]
    GameObject opponentAvatar;


    [SerializeField]
    TextMeshProUGUI wordsFound1;

    [SerializeField]
    TextMeshProUGUI wordsFound2;

    [SerializeField]
    private TextMeshProUGUI finalScore1;

    [SerializeField]
    private TextMeshProUGUI finalScore2;


    [SerializeField]
    private Transform scrollBarContent1;

    [SerializeField]
    private Transform scrollBarContent2;
    [SerializeField]
    CanvasGroup playerGroup;
    [SerializeField]
    CanvasGroup opponentGroup;
    [SerializeField]
    GameObject[] pSystems;
    [SerializeField]
    Transform bin;
    List<Player> players;
    Player current;

    public GameObject roomManager;

    public override void SetupAnimation()
    {
        background.DOColor(ColourMode.Instance.colour, 0);
        background.DOFade(0, 0);
        // statePrompt.DOFade(0, 0);
        // playerGroup.DOFade(0, 0);
        // opponentGroup.DOFade(0, 0);
        // newGameButton.GetComponent<Image>().DOFade(0, 0);
        // rematchButton.GetComponent<Image>().DOFade(0, 0);
        // rematchButton.transform.GetChild(0).GetComponent<Image>().DOFade(0, 0);
    }

    public override void Initialise()
    {
        base.Initialise();
        move = 0.5f;
        // check if online and move so one score with one word list
        newGameButton.onClick.AddListener(() =>
        {
            // ! no host migration change to use a server instead of client 
            // remove client 
            // InstanceFinder.ClientManager.StopConnection();
            // remove server if no players
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].GetComponent<NetworkObject>().IsOwner) current = players[i];
            }
            current.hasLeft = true;
            AudioManager.Instance.PlayPressed();
            Player.Instance.StopGame();
        });
        rematchButton.interactable = false;
        rematchButton.onClick.AddListener(() =>
        {
            // add more stuff here
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].GetComponent<NetworkObject>().IsOwner) current = players[i];
            }
            current.rematch = true;
            StartCoroutine(WaitForRematch());
            if (current.rematch && players[1 - players.IndexOf(current)].rematch)
            {
                AudioManager.Instance.PlayPressed();
                StartCoroutine(ExitAnimation());
            }

        });
    }

    public override void Update()
    {
        base.Update();

        if (!once)
        {
            once = true;
            players = GameManager.Instance.players.ToList();

            Player best;

            if (players[0].score > players[1].score)
            {
                best = players[0];
            }
            else if (players[1].score > players[0].score)
            {
                best = players[1];
            }
            else
            {
                best = null;
            }

            foreach (var p in pSystems) p.SetActive(false);
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].GetComponent<NetworkObject>().IsOwner)
                {
                    // if(Game.Instance.coop) good job
                    if (players[i] == best)
                    {
                        statePrompt.text = "You Win!";
                        statePrompt.characterSpacing = 18;
                        foreach (var p in pSystems) p.SetActive(true);
                    }
                    else if (best == null)
                    {
                        statePrompt.text = "You Drew";
                        statePrompt.characterSpacing = 15;
                    }
                    else
                    {
                        statePrompt.text = "You Lose..";
                        statePrompt.characterSpacing = 10;
                    }
                    playerAvatar.transform.GetChild(0).GetComponent<Image>().color = players[i].GetComponent<PlayerSettings>().GetColour();
                    playerAvatar.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].GetComponent<PlayerSettings>().GetUsername().ToUpper()[0].ToString();
                    playerAvatar.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = ColourMode.Instance.avatarText;
                    playerAvatar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = players[i].GetComponent<PlayerSettings>().GetUsername();
                    finalScore1.text = players[i].score.ToString("D3");
                    wordsFound1.text = players[i].usedWords.Count.ToString();
                    // * add to highscores to leaderboards
                    // get client words
                    foreach (var word in players[i].usedWords) Utility.CreateWord(scrollBarContent1, word, font);
                    // * check for highscoring words and replace in personal leaderboard and all time
                }

                else
                {
                    opponentAvatar.transform.GetChild(0).GetComponent<Image>().color = players[i].GetComponent<PlayerSettings>().GetColour();
                    opponentAvatar.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].GetComponent<PlayerSettings>().GetUsername().ToUpper()[0].ToString();
                    opponentAvatar.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = ColourMode.Instance.avatarText;
                    opponentAvatar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = players[i].GetComponent<PlayerSettings>().GetUsername();
                    wordsFound2.text = players[i].usedWords.Count.ToString();
                    finalScore2.text = players[i].score.ToString("D3");
                    // get opponents words
                    foreach (var word in players[i].usedWords) Utility.CreateWord(scrollBarContent2, word, font);
                }
                players[i].UpdatePlayer(0, new(), new(), 0);
            }
        }
    }
    public override IEnumerator Animate()
    {
        background.DOFade(1, move);
        yield return new WaitForSeconds(move / 2);
        // statePrompt.DOFade(1, move);
        // yield return new WaitUntil(() => !DOTween.IsTweening(background));
        // playerGroup.DOFade(1, move);
        // yield return new WaitForSeconds(move / 2);
        // opponentGroup.DOFade(1, move);
        // yield return new WaitUntil(() => !DOTween.IsTweening(opponentGroup));
        // newGameButton.GetComponent<Image>().DOFade(1, move);
        // rematchButton.GetComponent<Image>().DOFade(1, move);
        // rematchButton.transform.GetChild(0).GetComponent<Image>().DOFade(1, move);
    }

    public IEnumerator ExitAnimation()
    {
        background.DOColor(Color.white, move);
        // statePrompt.DOFade(1, move);
        // playerGroup.DOFade(1, move);
        // opponentGroup.DOFade(1, move);
        // newGameButton.GetComponent<Image>().DOFade(1, move);
        // rematchButton.GetComponent<Image>().DOFade(1, move);
        // rematchButton.transform.GetChild(0).GetComponent<Image>().DOFade(1, move);
        yield return new WaitUntil(() => !DOTween.IsTweening(background));
        // Player.Instance.StopGame();
        // LobbyNetwork.LeaveRoom();
        for (int j = 0; j < scrollBarContent2.childCount; j++) scrollBarContent2.GetChild(j).SetParent(bin);
        for (int i = 0; i < scrollBarContent1.childCount; i++) scrollBarContent1.GetChild(i).SetParent(bin);
        ViewManager.Instance.Show<TitleView>();
    }

    IEnumerator WaitForRematch()
    {
        int index = players.IndexOf(current);
        yield return new WaitUntil(() => players[1 - index].rematch || players[1 - index].hasLeft);
        if (current.rematch && players[1 - index].rematch)
        {
            // create new room with these two with same settings
            Room room = null;
            foreach (Transform r in roomManager.transform)
            {
                if (Player.Instance.roomID == r.GetComponent<Room>().roomName)
                {
                    room = r.GetComponent<Room>();
                    break;
                }
            }
            string roomId = Utility.GenerateRandomString(12);
            if (players[0].IsOwner && room != null) LobbyNetwork.CreateRoom(roomId, roomId, room.min, room.max, room.time);
            else if (players[1].IsOwner && room != null)
            {
                yield return new WaitForSeconds(2f);
                foreach (Transform r in roomManager.transform)
                {
                    room = r.GetComponent<Room>();
                    if (room.roomName == room.password)
                    {
                        LobbyNetwork.JoinRoom(room.roomName, room.roomName);
                    }
                }
            }
            else
            {
                print("failed");
                StartCoroutine(ExitAnimation());
            }
        }
        else
        {
            StartCoroutine(ExitAnimation());
        }

    }
}