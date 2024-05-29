using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FirstGearGames.LobbyAndWorld.Lobbies;
using FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases;
using FirstGearGames.LobbyAndWorld.Clients;

public class Player : NetworkBehaviour
{
    public static Player Instance { get; private set; }

    [field: SyncVar]
    public bool canStart
    {
        get;

        [ServerRpc(RequireOwnership = false)]
        set;
    }
    [field: SyncVar]
    public bool rematch
    {
        get;

        [ServerRpc(RequireOwnership = false)]
        set;
    }
    public bool hasLeft
    {
        get;

        [ServerRpc(RequireOwnership = false)]
        set;
    }
    [field: SyncVar]
    public string roomID { get; set; }
    public int score;
    public string gameSelected;
    public List<string> allWords;
    // public List<string> usedWords = new();
    [field: SyncObject]
    public SyncList<string> usedWords {get;} = new();
    public int allWordsScore;
    public bool played = false;
    [field: SyncVar]
    public PlayerData data;
    bool loaded = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner) return;

        Instance = this;
        ObjectFinder.Instance.GetAllGameObjects();
        GameObject parentObj = ObjectFinder.Instance.FindInAll("PlayersHolder");
        transform.parent = parentObj.transform;
        data = new(null, ColourMode.Instance.colour, AudioManager.Instance);
        if (SaveSystem.LoadFromJson<PlayerData>() == null)
        {
            SaveSystem.SaveToJson(data);
            loaded = true;
        }
        if (!loaded) data = SaveSystem.LoadFromJson<PlayerData>();
        ViewManager.Instance.Initialise();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        SaveSystem.SaveToJson(new PlayerData(GetComponent<PlayerSettings>(), ColourMode.Instance.colour, AudioManager.Instance));
    }

    [Server]
    public void StartGame()
    {
        TargetStartGame(Owner);
    }

    [TargetRpc]
    private void TargetStartGame(NetworkConnection networkConnection)
    {
        score = 0;
        hasLeft = false;
        // show match view?
        usedWords.Clear();
        allWords.Clear();
        allWordsScore = 0;
        // StartCoroutine(ShowSettingsBtn(loadTime));
    }

    [Client]
    public void StopGame()
    {
        StartCoroutine(ViewManager.Instance.currentView.GetComponent<GameOverView>().ExitAnimation());
        // foreach (string word in Finder.Instance.usedWords) GameManager.Instance.RemoveWord(word);
        GameManager.Instance.players.Remove(this);
    }

    // [Server]
    // public void CmdStopGame()
    // {
    //     TargetStopGame(Owner);
    // }
    
    // [TargetRpc]
    // void TargetStopGame(NetworkConnection conn){
    //     StartCoroutine(ViewManager.Instance.currentView.GetComponent<GameOverView>().ExitAnimation());
    //     foreach (string word in Finder.Instance.usedWords) RemoveWord(word);
    //     GameManager.Instance.players.Remove(this);
    // }

    public void Rematch()
    {
        // add to rematch queue 
        // if both players agree 
        // check if one disagrees
        // then kick other player to title after time

    }
    
    private void Update() {
        if(Player.Instance != null) print(string.Join(" ", Player.Instance.usedWords));
    }
    // [TargetRpc]
    // public void TargetPlayerUpdate(NetworkConnection networkConnection){
    //     PlayerUpdate();
    // }
    // public void PlayerUpdate(){
    //     score = Game.Instance.score;
    //     foreach (string word in Finder.Instance.usedWords) usedWords.Add(word);
    // }
    [Client]
    public void UpdatePlayer(int _score, List<string> _usedWords, List<string> _allWords, int _allWordsScore)
    {
        UpdatePlayers(_score, _usedWords, _allWords, _allWordsScore);
    }

    [Server]
    private void UpdatePlayers(int _score, List<string> _usedWords, List<string> _allWords, int _allWordsScore){
        CmdUpdatePlayers(_score, _usedWords, _allWords, _allWordsScore);
    }

    [ServerRpc]
    private void CmdUpdatePlayers(int _score, List<string> _usedWords, List<string> _allWords, int _allWordsScore)
    {
        UpdateScore(_score);
        UpdateUsedWords(_usedWords);
        UpdateAllWords(_allWords, _allWordsScore);
    }

    // [Server]
    [ObserversRpc(BufferLast = true)]
    public void UpdateScore(int _score)
    {
        score = _score;
    }

    // [ObserversRpc(BufferLast = true)]
    [Server]
    public void UpdateUsedWords(List<string> _usedWords)
    {
        foreach (string word in _usedWords) usedWords.Add(word);
    }

    [ObserversRpc(BufferLast = true)]
    public void UpdateAllWords(List<string> _allWords, int _score)
    {
        allWordsScore = _score;
        allWords = _allWords;
    }

    IEnumerator ShowSettingsBtn(float waitTime)
    {
        GameObject settingsbtn = ObjectFinder.Instance.FindInAll("Settings");
        yield return new WaitForSeconds(waitTime);
        settingsbtn.SetActive(true);
    }


    [Server]
    public void AddWord(string word){
        usedWords.Add(word);
    }

    [Server]
    public void RemoveWord(string word){
        usedWords.Remove(word);
    }
}