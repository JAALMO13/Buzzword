// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using FishNet.Object;
// using FishNet.Object.Synchronizing;

// [System.Serializable]
// public class Match{
//     public string matchID;
//     public SyncListGameObject players = new();
//     public GameSettings settings;
//     public string password;
//     public Match(){}
//     public Match(string matchID, GameObject player, GameSettings settings, string password=""){
//         this.matchID = matchID;
//         this.settings = settings;
//         players.Add(player);
//     }
// }

// [System.Serializable]
// public class SyncListGameObject : SyncList<GameObject>{}

// [System.Serializable]
// public class SyncListMatch : SyncList<Match>{}

// public class MatchMaker : MonoBehaviour
// {
//     public static MatchMaker Instance {get; private set;}
//     public SyncListMatch regular = new();
//     public SyncListMatch friends = new();
//     public SyncListMatch rematch = new();
    
//     private void Awake(){
//         Instance = this;
//     }
//     public Match CheckSettings(SyncListMatch queue, GameSettings gameSettings){
//         for(int i = 0; i < queue.Count; i++){
//             if(queue[i].settings == gameSettings){
//                 return queue[i];
//             }
//         }
//         return null;
//     }

//     public void CreateMatch(SyncListMatch queue, GameObject player, GameSettings settings, string password = ""){
//         queue.Add(new(Utility.GenerateRandomString(12), player, settings, password));
//     }
// }
