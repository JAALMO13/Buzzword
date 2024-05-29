using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using System.Collections;
using System.Threading;
using FirstGearGames.LobbyAndWorld.Lobbies;
using FirstGearGames.LobbyAndWorld.Clients;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject letterPrefab;
    [field: SyncVar]
    public string sharedWord;
    [field: SyncVar]
    public bool canStart;
    [field: SyncVar]
    public bool didStart;
    [field: SyncVar]
    public string gameMode;
    [field: SyncObject]
    public SyncList<Player> players { get; } = new SyncList<Player>();

    [field: SyncObject]
    public SyncList<string> allWords{get;} = new SyncList<string>();
    public int allWordsScore;
    
    public Game game;

    public GameObject roomActionCanvas;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        List<NetworkObject> members = LobbyNetwork.CurrentRoom.MemberIds;
        for(int i = 0; i < members.Count; i++){
            players.Add(members[i].GetComponent<Player>());
        }
        // foreach(Player p in players) p.usedWords.Clear();
        ObjectFinder.Instance.GetAllGameObjects();
        gameMode = "Anagram";
        game = ObjectFinder.Instance.FindInAll(gameMode + "Game").GetComponent<Game>();
        game.gameObject.SetActive(true);
        // roomActionCanvas = ObjectFinder.Instance.FindInAll("RoomActionCanvas");
        // roomActionCanvas.SetActive(false);
    }
    public void StartGame(){
        CmdStartGame(this);
    }
    [Server]
    public void CmdStartGame(GameManager script){
        script.ServerStartGame();
    }

    
    public void ServerStartGame()
    {
        
        Finder.Instance.ResetLists();
        
        sharedWord = WordGenerator.randomWord(8); 
        Game.Instance.originalWord = sharedWord;
        Thread bgThread = new Thread(new ThreadStart(AllWords));
        bgThread.Start();

        
        didStart = true;
        for (int i = 0; i < 8; i++)
        {
           SpawnLetter(i);
        }
        Player.Instance.played = true;
        Game.Instance.state = 0;

        // check the two players
        for (int i = 0; i < players.Count; i++)
        {
            players[i].StartGame();
        }
    }
    
    [ServerRpc(RequireOwnership=false)]
    public void SpawnLetter(int id)
    {
        GameObject go = Instantiate(letterPrefab, new Vector3(1000f, 1000f, 1000f), Quaternion.identity);
        ServerManager.Spawn(go);
    }
    [ServerRpc(RequireOwnership=false)]
    public void DespawnLetters(){
        foreach(GameObject letter in Game.Instance.letterPrefabs){
            ServerManager.Despawn(letter);
        }
    }

    [ServerRpc(RequireOwnership=false)]
    void CmdShowPrefabs(int i){
        StartCoroutine(ShowPrefabs(i));
    }

    IEnumerator ShowPrefabs(int i)
    {
        yield return new WaitForSeconds(0.3f);
        Game.Instance.letterPrefabs[i].SetActive(true);
    }


    void AllWords()
    {
        Finder.Instance.GenAllPossible(GameManager.Instance.sharedWord);
        allWords.AddRange(Finder.Instance.allPossibleWords);
        allWordsScore = Calculator.WordTotalList(allWords.ToList(), game.minLength);
    }



}

/*
TODO:
make offline game manager too
make scripts accessible for both online and offline

*/
