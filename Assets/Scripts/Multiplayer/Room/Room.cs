using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FirstGearGames.LobbyAndWorld.Lobbies;
using FishNet;

public class Room : NetworkBehaviour
{
    [SyncVar]
    public string gameMode = "Anagrams";
    [SyncVar]
    public string roomName;
    [SyncVar]
    public string password;
    [SyncVar]
    public int min;
    [SyncVar]
    public int max;
    [SyncVar]
    public int time;
    [SyncVar]
    public int playerCount;
    [SyncVar]
    public bool used;
    bool done;
    private void Start() {
        Transform parentobj = ObjectFinder.Instance.FindInAll("RoomManager").transform;
        transform.parent = parentobj;
        gameMode = "Anagrams";
    }

    private void Update() {

        if(LobbyNetwork.CurrentRoom == null) return;
        // if(LobbyNetwork.CurrentRoom.MemberIds.Count == 0) return;
        playerCount = LobbyNetwork.CurrentRoom.MemberIds.Count;
        if(playerCount > 1) used = true;
        if(playerCount == 0 && used) ServerDespawnRoom();
    }

    [ServerRpc(RequireOwnership =false)]
    void ServerDespawnRoom(){
        StartCoroutine(DespawnRoom());
    }

    IEnumerator DespawnRoom(){
        if(!done){
            done = true;
            yield return new WaitForSeconds(0.5f);
            ServerManager.Despawn(gameObject);
        }
    }
}
