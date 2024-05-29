using FirstGearGames.LobbyAndWorld.Demos.KingOfTheHill;
using FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace FirstGearGames.LobbyAndWorld.Lobbies
{

    public class ReadyLobbyNetwork : LobbyNetwork
    {
        #region Public.
        /// <summary>
        /// Called when a member of your current room changes their ready state.
        /// </summary>
        public static event Action<NetworkObject, bool> OnMemberSetReady;
        #endregion

        #region Private.
        /// <summary>
        /// Players which have readied for each room.
        /// </summary>
        private Dictionary<RoomDetails, HashSet<NetworkObject>> _serverReadyPlayers = new Dictionary<RoomDetails, HashSet<NetworkObject>>();
        /// <summary>
        /// Players which have readied for local players current room.
        /// </summary>
        private HashSet<NetworkObject> _clientReadyPlayers = new HashSet<NetworkObject>();
        /// <summary>
        /// Singleton instance to this script.
        /// </summary>
        private static new ReadyLobbyNetwork _instance;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _instance = this;
            base.OnClientLeftRoom += MyLobbyNetwork_OnClientLeftRoom;
            base.OnClientJoinedRoom += ReadyLobbyNetwork_OnClientJoinedRoom;
            InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;

            LobbyNetwork.OnMemberLeft += LobbyNetwork_OnMemberLeft;
            InstanceFinder.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }


        private void OnDestroy()
        {
            if (InstanceFinder.ClientManager == null)
                return;
            InstanceFinder.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
            LobbyNetwork.OnMemberLeft -= LobbyNetwork_OnMemberLeft;
        }


        /// <summary>
        /// Called after the local client connection state changes.
        /// </summary>
        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Started)
                return;

            //Clear local ready players.
            _clientReadyPlayers.Clear();

            /* Unload all scenes except lobby for client.
             * Since they are disconnected this doesn't ened to be done through
             * FSM. Also, FSM scene changes MUST only be done via server,
             * this is being run on client. */
            for (int i = 0; i < UnitySceneManager.sceneCount; i++)
            {
                Scene s = UnitySceneManager.GetSceneAt(i);
                if (s != gameObject.scene)
                    UnitySceneManager.UnloadSceneAsync(s);
            }

            base.LobbyCanvases.SetLobbyCameraActive(true);
        }

        #region SceneManager callbacks.
        /// <summary>
        /// Called after SceneManager has loaded a scene.
        /// </summary>
        /// <param name="obj"></param>
        private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj)
        {
            if (!obj.QueueData.AsServer)
                return;

            /* When the server loads a scene try to find the SceneRoomDetails script in it
             * and pass in the RoomDetails for the scene loaded. This isn't required by any
             * means but it shows how you can have a reference to the RoomDetails which
             * the scene is for on the server. */
            object[] p = obj.QueueData.SceneLoadData.Params.ServerParams;
            if (p != null && p.Length > 1)
            {
                RoomDetails rd = (RoomDetails)p[1];
                //Try to find script in scene.
                foreach (Scene s in obj.LoadedScenes)
                {
                    GameObject[] gos = s.GetRootGameObjects();
                    for (int i = 0; i < gos.Length; i++)
                    {
                        //If found.
                        if (gos[i].TryGetComponent<GameplayManager>(out GameplayManager gpm))
                        {
                            gpm.FirstInitialize(rd, this);
                            break;
                        }

                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Called when a client joins a room. This is called after the client has been sent a successful join response.
        /// </summary>
        private void ReadyLobbyNetwork_OnClientJoinedRoom(RoomDetails roomDetails, NetworkObject joiner)
        {
            HashSet<NetworkObject> readyPlayers;
            //If any players have readied up send it to joining player.
            if (_serverReadyPlayers.TryGetValue(roomDetails, out readyPlayers))
            {
                foreach (NetworkObject item in readyPlayers)
                    TargetSetReady(joiner.Owner, item, true);
            }
        }


        /// <summary>
        /// Tries ready state over the network for local client.
        /// </summary>
        /// <param name="ready"></param>
        [Client]
        public static void SetReady(bool ready)
        {
            _instance.SetReadyInternal(ready);
        }
        private void SetReadyInternal(bool ready)
        {
            CmdSetReady(ready);
        }

        /// <summary>
        /// Sets ready state for player in their room.
        /// </summary>
        /// <param name="sender"></param>
        [ServerRpc(RequireOwnership = false)]
        private void CmdSetReady(bool ready, NetworkConnection sender = null)
        {
            SetReady(ready, sender.FirstObject, true);
        }
         
        /// <summary>
        /// Sets ready state for a client.
        /// </summary>
        /// <param name="changingPlayer"></param>
        private void SetReady(bool ready, NetworkObject changingPlayer, bool asServer)
        {
            //Running on server.
            if (asServer)
            {
                RoomDetails roomDetails;
                //Not in a room.
                if (!base.ConnectionRooms.TryGetValue(changingPlayer.Owner, out roomDetails))
                {
                    Debug.LogWarning($"Cannot ready client as they are not in a room.");
                }
                //In a room, find room in ready list and add player.
                else
                {
                    HashSet<NetworkObject> readyPlayers;
                    //If not found make new hashset.
                    if (!_serverReadyPlayers.TryGetValue(roomDetails, out readyPlayers))
                    {
                        readyPlayers = new HashSet<NetworkObject>();
                        _serverReadyPlayers[roomDetails] = readyPlayers;
                    }

                    if (ready)
                        readyPlayers.Add(changingPlayer);
                    else
                        readyPlayers.Remove(changingPlayer);

                    foreach (NetworkObject item in roomDetails.MemberIds)
                        TargetSetReady(item.Owner, changingPlayer, ready);
                }
            }
            //Running on client.
            else
            {
                if (ready)
                    _clientReadyPlayers.Add(changingPlayer);
                else
                    _clientReadyPlayers.Remove(changingPlayer);

                OnMemberSetReady?.Invoke(changingPlayer, ready);
            }
        }

        /// <summary>
        /// Tells a client to set a player ready or not ready.
        /// Used so that players can visualize who has readied, and who has not.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="identity"></param>
        /// <param name="ready"></param>
        [TargetRpc]
        private void TargetSetReady(NetworkConnection conn, NetworkObject identity, bool ready)
        {
            SetReady(ready, identity, false);
        }

        /// <summary>
        /// Called on server when a client leaves a room.
        /// </summary>
        /// <param name="roomDetails"></param>
        /// <param name="leaver"></param>
        private void MyLobbyNetwork_OnClientLeftRoom(RoomDetails roomDetails, NetworkObject leaver)
        {
            //If no more members in room then try to remove from readyplayers.
            if (roomDetails.MemberIds.Count == 0)
            {
                _serverReadyPlayers.Remove(roomDetails);
            }
            //If still has members then remove leaving member.
            else
            {
                if (_serverReadyPlayers.TryGetValue(roomDetails, out HashSet<NetworkObject> idents))
                    idents.Remove(leaver);
            }
        }

        /// <summary>
        /// Called on client when a member leaves their current room.
        /// </summary>
        /// <param name="obj"></param>
        private void LobbyNetwork_OnMemberLeft(NetworkObject obj)
        {
            /* If self that left then clear ready players.
             * Otherwise remove leaving player. */
            if (obj == InstanceFinder.ClientManager.Connection.FirstObject)
                _clientReadyPlayers.Clear();
            else
                _clientReadyPlayers.Remove(obj);
        }

        /// <summary>
        /// Returns if room can start. Can be used to create your own start room conditions.
        /// </summary>
        /// <param name="roomDetails">RoomDetails for the game trying to be started.</param>
        /// <param name="startingPlayer">Player which is trying to start the game.</param>
        /// <param name="failedReason">Fill this string with the failed reason if not able to start game.</param>
        protected override bool OnCanStartRoom(RoomDetails roomDetails, NetworkObject startingPlayer, ref string failedReason, bool asServer)
        {
            //Something in base script prevented starting.
            if (!base.OnCanStartRoom(roomDetails, startingPlayer, ref failedReason, asServer))
                return false;

            //Not enough members to start room.
            if (roomDetails.MemberIds.Count < 1)
            {
                failedReason = "Not enough members to start.";
                return false;
            }

            /* If match has already started and this far then lock on start is false,
             * meaning players can join and leave as they wish. At this point there is
             * no reason to require players ready since the game started. */
            if (roomDetails.IsStarted)
                return true;

            //Try to get current ready for this room. If doesn't exist yet then make.
            HashSet<NetworkObject> readyPlayers;
            //If running as server then use server hashset.
            if (asServer)
            {
                if (!_serverReadyPlayers.TryGetValue(roomDetails, out readyPlayers))
                {
                    readyPlayers = new HashSet<NetworkObject>();
                    _serverReadyPlayers[roomDetails] = readyPlayers;
                }
            }
            //Otherwise use client hashset.
            else
            {
                readyPlayers = _clientReadyPlayers;
            }

            /* In the base class only host can start the room; though, you
            * very well could override that. For this example we will not,
            * and it would be redundant to make host ready while also clicking
            * start. Instead I will check if startingPlayer is host, and if so,
            * automatically add them to ready players. */
            if (roomDetails.MemberIds[0] == startingPlayer)
                SetReady(true, startingPlayer, asServer);

            //Ready players count is same as member count for room.
            if (readyPlayers.Count == roomDetails.MemberIds.Count)
            {
                return true;
            }
            else
            {
                failedReason = "Not all players are ready.";
                return false;
            }
        }

    }
}

