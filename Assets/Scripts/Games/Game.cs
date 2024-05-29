using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Object;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using FirstGearGames.LobbyAndWorld.Lobbies;
#pragma warning restore format

// todo exit and settings button


public abstract class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    [HideInInspector] public Dictionary<string, int> settings = new();
    [HideInInspector] public bool online = true;
    string gameMode;
    [HideInInspector] public GameObject lettersParent;
    [HideInInspector] public GameObject cover;
    public GameObject letterPrefab;
    public int minLength = 3;
    public int length = 6;
    [HideInInspector] public int score;
    [HideInInspector]
    public List<GameObject> letterPrefabs = new List<GameObject>();
    public string originalWord;
    [HideInInspector] public string word;
    public bool useTimer;
    public float gameLength = 60;
    float elapsedTime = 0;
    bool run = false;
    bool once = false;
    bool initialised = false;
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public string currentView;
    [HideInInspector] public int state { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public virtual void Initialise()
    {

        gameMode = ObjectFinder.Instance.FindInAllTag("Game").name;
        GameObject roomManager = ObjectFinder.Instance.FindInAll("RoomManager");
        Room room = null;
        foreach (Transform r in roomManager.transform){
            if(Player.Instance.roomID == r.GetComponent<Room>().roomName){
                room = r.GetComponent<Room>();
                break;
            }
        }
        currentView = gameMode + "View";
        lettersParent = ObjectFinder.Instance.FindInAll("LettersBox");
        cover = ObjectFinder.Instance.FindInAll("Cover");
        cover.SetActive(true);
        // ! if offline mode selected deactivate networkmanager and gamemanager and lettersParent
        online = ObjectFinder.Instance.FindInAll("NetworkManager").activeSelf;
        length = room.max;
        minLength = room.min;
        gameLength = room.time;
        if (length < 3)
        {
            length = 3;
            minLength = 2;
        }
        
    }

    public virtual void Update()
    {
        if (!initialised)
        {
            Initialise();
            initialised = true;
        }
        if (letterPrefabs.Count == 0)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 2)
            {
                letterPrefabs = GameObject.FindGameObjectsWithTag("Letter").ToList();
                elapsedTime = 0;
            }
        }

        if (useTimer && ViewManager.Instance.currentView.GetType().ToString() == currentView)
        {
            if (Timer.current.getTime() <= 0 && !run)
            {
                // restart timer
                Timer.current.SetTime(gameLength);
                Timer.current.SetIsCountdown(true);
                run = true;
            }
            if (GameManager.Instance.players[0].canStart && GameManager.Instance.players[1].canStart)
            {
                StartCoroutine(GameStart());
            }

            // game finished 
            if (Timer.current.getTime() <= 0 && run && !once)
            {
                
                StartCoroutine(GameOver());
            }

        }

    }
    
    public virtual void GenerateWord(int len)
    {
        if (!online)
        {
            for (int i = 0; i < length; i++)
            {
                // instantiate gameobject 
                GameObject go = Instantiate(letterPrefab, new Vector3(0, 0, 0), transform.rotation);
                go.GetComponent<NetworkObject>().enabled = false;
                // if(online) {
                //     go.GetComponent<NetworkObject>().enabled = true;
                //     GameManager.Instance.SpawnLetter(go);
                // }
                letterPrefabs.Add(go);
            }
        }
    }

    IEnumerator GameOver()
    {
        once = true;
        
        // Player.Instance.UpdatePlayer(score, Finder.Instance.usedWords, GameManager.Instance.allWords.ToList(), GameManager.Instance.allWordsScore);
        Player.Instance.UpdateScore(score);
        // Player.Instance.usedWords = GameManager.Instance.usedWords.ToList();

        // Player.Instance.UpdateUsedWords(Player.Instance.usedWords);
        
        gameOver = true; 
        yield return new WaitUntil(() => Player.Instance.score >= 0);
        foreach (GameObject p in letterPrefabs) p.SetActive(false);

        LobbyNetwork.LeaveRoom();  
    }

    IEnumerator GameStart(){
        yield return new WaitForSeconds(2f);
        cover.transform.GetChild(0).GetComponent<Image>().DOFade(0, 0.3f);
        for(int i = 0; i < cover.transform.GetChild(1).childCount; i++) cover.transform.GetChild(1).GetChild(i).GetComponent<Image>().DOFade(0, 0.3f);
        cover.GetComponent<Image>().DOFade(0, 0.4f).OnComplete(() =>
        {
            Timer.current.startTimer();
            
            // cover.SetActive(false);
        });
    }
}
