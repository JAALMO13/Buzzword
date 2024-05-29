using FirstGearGames.LobbyAndWorld.Clients;
using FirstGearGames.LobbyAndWorld.Global;
using FirstGearGames.LobbyAndWorld.Global.Canvases;
using FirstGearGames.LobbyAndWorld.Lobbies;
using FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace FirstGearGames.LobbyAndWorld.Demos.KingOfTheHill
{


    public class GameplayManager : NetworkBehaviour
    {
        #region Serialized
        [Header("Spawning")]
        /// <summary>
        /// Region players may spawn.
        /// </summary>
        [Tooltip("Region players may spawn.")]
        [SerializeField]
        private Vector3 _spawnRegion = Vector3.one;
        /// <summary>
        /// Prefab to spawn.
        /// </summary>
        [Tooltip("Prefab to spawn.")]
        [SerializeField]
        private NetworkObject _playerPrefab = null;
        /// <summary>
        /// DeathDummy to spawn.
        /// </summary>
        [Tooltip("DeathDummy to spawn.")]
        [SerializeField]
        private GameObject _deathDummy = null;
        #endregion

        /// <summary>
        /// RoomDetails for this game. Only available on the server.
        /// </summary>
        private RoomDetails _roomDetails = null;
        /// <summary>
        /// LobbyNetwork.
        /// </summary>
        private LobbyNetwork _lobbyNetwork = null;
        /// <summary>
        /// Becomes true once someone has won.
        /// </summary>
        private bool _winner = false;
        /// <summary>
        /// Currently spawned player objects. Only exist on the server.
        /// </summary>
        private List<NetworkObject> _spawnedPlayerObjects = new List<NetworkObject>();

        #region Initialization and Deinitialization.
        private void OnDestroy()
        {
            if (_lobbyNetwork != null)
            {
                _lobbyNetwork.OnClientJoinedRoom -= LobbyNetwork_OnClientStarted;
                _lobbyNetwork.OnClientLeftRoom -= LobbyNetwork_OnClientLeftRoom;
            }
        }

        /// <summary>
        /// Initializes this script for use.
        /// </summary>
        public void FirstInitialize(RoomDetails roomDetails, LobbyNetwork lobbyNetwork)
        {
            _roomDetails = roomDetails;
            _lobbyNetwork = lobbyNetwork;
            _lobbyNetwork.OnClientStarted += LobbyNetwork_OnClientStarted;
            _lobbyNetwork.OnClientLeftRoom += LobbyNetwork_OnClientLeftRoom;
        }

        /// <summary>
        /// Called when a client leaves the room.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void LobbyNetwork_OnClientLeftRoom(RoomDetails arg1, NetworkObject arg2)
        {
            //Destroy all of clients objects, except their client instance.
            for (int i = 0; i < _spawnedPlayerObjects.Count; i++)
            {
                NetworkObject entry = _spawnedPlayerObjects[i];
                //Entry is null. Remove and iterate next.
                if (entry == null)
                {
                    _spawnedPlayerObjects.RemoveAt(i);
                    i--;
                    continue;
                }

                //If same connection to client (owner) as client instance of leaving player.
                if (_spawnedPlayerObjects[i].Owner == arg2.Owner)
                {
                    //Destroy entry then remove from collection.
                    entry.Despawn();
                    _spawnedPlayerObjects.RemoveAt(i);
                    i--;                        
                }

            }
        }

        /// <summary>
        /// Called when a client starts a game.
        /// </summary>
        /// <param name="roomDetails"></param>
        /// <param name="client"></param>
        private void LobbyNetwork_OnClientStarted(RoomDetails roomDetails, NetworkObject client)
        {
            //Not for this room.
            if (roomDetails != _roomDetails)
                return;
            //NetIdent is null or not a player.
            if (client == null || client.Owner == null)
                return;

            /* POSSIBLY USEFUL INFORMATION!!!!!
             * POSSIBLY USEFUL INFORMATION!!!!!
             * If you want to wait until all players are in the scene
             * before spaning then check if roomDetails.StartedMembers.Count
             * is the same as roomDetails.MemberIds.Count. A member is considered
             * started AFTER they have loaded all of the scenes. */
            SpawnPlayer(client.Owner);
        }
        #endregion

        #region Death.
        /// <summary>
        /// Called when object exits trigger. Used to respawn players.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (!base.IsServer)
                return;

            NetworkObject netIdent = other.gameObject.GetComponent<NetworkObject>();
            //If doesn't have a netIdent or no owning client exit.
            if (netIdent == null || netIdent.Owner == null)
                return;

            //If there is an owning client then destroy the object and respawn.
            StartCoroutine((__DelayRespawn(netIdent)));
        }

        /// <summary>
        /// Destroys netIdent and respawns player after delay.
        /// </summary>
        /// <param name="netIdent"></param>
        /// <returns></returns>
        private IEnumerator __DelayRespawn(NetworkObject netIdent)
        {
            //Send Rpc to spawn death dummy then destroy original.
            RpcSpawnDeathDummy(netIdent.transform.position);
            NetworkConnection conn = netIdent.Owner;
            InstanceFinder.ServerManager.Despawn(netIdent.gameObject);

            //Wait a little to respawn player.
            yield return new WaitForSeconds(1f);
            //Don't respawn if someone won.
            if (_winner)
                yield break;
            /* Check for rage quit conditions (left room). */
            if (conn == null)
                yield break;
            ClientInstance ci = ClientInstance.ReturnClientInstance(conn);
            if (ci == null || !_roomDetails.StartedMembers.Contains(ci.NetworkObject))
                yield break;

            SpawnPlayer(conn);
        }

        /// <summary>
        /// Spawns a dummy player to show death.
        /// </summary>
        /// <param name="player"></param>
        [ObserversRpc]
        private void RpcSpawnDeathDummy(Vector3 position)
        {
            GameObject go = Instantiate(_deathDummy, position, Quaternion.identity);
            UnitySceneManager.MoveGameObjectToScene(go, gameObject.scene);
            Destroy(go, 1f);
        }
        #endregion

        #region Winning.
        /// <summary>
        /// Called when a player wins.
        /// </summary>
        private void KingTimer_OnTimerComplete(NetworkObject winner)
        {
            //Already a winner. Could happen if both clients somehow managed to win at the exact same frame.
            if (_winner)
                return;
            _winner = true;

            StartCoroutine(__PlayerWon(winner));
        }

        /// <summary>
        /// Ends the game announcing winner and sending clients back to lobby.
        /// </summary>
        /// <returns></returns>
        private IEnumerator __PlayerWon(NetworkObject winner)
        {
            //Find all players in room and destroy their objects. Don't destroy client instance!
            foreach (NetworkObject item in _roomDetails.StartedMembers)
            {
                //If not winner.
                if (item.Owner != winner.Owner)
                {
                    foreach (NetworkObject ni in item.Owner.Objects)
                    {
                        //Try to get king timer, if object with timer then stop timer.
                        if (ni.TryGetComponent<KingTimer>(out KingTimer kt))
                            kt.StopTimer();
                    }
                }
            }

            //Send out winner text.
            ClientInstance ci = ClientInstance.ReturnClientInstance(winner.Owner);
            string playerName = ci.PlayerSettings.GetUsername();
            foreach (NetworkObject item in _roomDetails.StartedMembers)
            {
                if (item != null && item.Owner != null)
                    TargetShowWinner(item.Owner, playerName, (item.Owner == winner.Owner));
            }

            //Wait a moment then kick the players out. Not required.
            yield return new WaitForSeconds(4f);
            List<NetworkObject> collectedIdents = new List<NetworkObject>();
            foreach (NetworkObject item in _roomDetails.StartedMembers)
            {
                ClientInstance cli = ClientInstance.ReturnClientInstance(item.Owner);
                if (ci != null)
                    collectedIdents.Add(cli.NetworkObject);
            }
            foreach (NetworkObject item in collectedIdents)
                _lobbyNetwork.TryLeaveRoom(item);
        }

        /// <summary>
        /// Displayers who won.
        /// </summary>
        /// <param name="winner"></param>
        [TargetRpc]
        private void TargetShowWinner(NetworkConnection conn, string winnerName, bool won)
        {
            Color c = (won) ? MessagesCanvas.LIGHT_BLUE : Color.red;
            string text = (won) ? "Congrats, you won!" :
                $"{winnerName} has won, better luck next time!";
            GlobalManager.CanvasesManager.MessagesCanvas.InfoMessages.ShowTimedMessage(text, c, 4f);
        }
        #endregion

        #region Spawning.
        /// <summary>
        /// Spawns a player at a random position for a connection.
        /// </summary>
        /// <param name="conn"></param>
        private void SpawnPlayer(NetworkConnection conn)
        {
            //Move the player randomly within spawn region.
            float x = Random.Range(-_spawnRegion.x / 2f, _spawnRegion.x / 2f);
            float y = Random.Range(-_spawnRegion.y / 2f, _spawnRegion.y / 2f);
            float z = Random.Range(-_spawnRegion.z / 2f, _spawnRegion.z / 2f);
            Vector3 next = transform.position + new Vector3(x, y, z);

            //Make object and move it to proper scene.
            NetworkObject nob = Instantiate<NetworkObject>(_playerPrefab, next, Quaternion.identity);
            UnitySceneManager.MoveGameObjectToScene(nob.gameObject, gameObject.scene);

            _spawnedPlayerObjects.Add(nob);
            //Subscriber to kingtimer so we know when a player reaches 0.
            KingTimer kt = nob.GetComponentInChildren<KingTimer>();
            kt.OnTimerComplete += KingTimer_OnTimerComplete;
            base.Spawn(nob.gameObject, conn);

            //NetworkObject netIdent = conn.identity;            
            nob.transform.position = next;
            ObserversTeleport(nob, next);
        }
        /// <summary>
        /// teleports a NetworkObject to a position.
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="position"></param>
        [ObserversRpc]
        private void ObserversTeleport(NetworkObject ident, Vector3 position)
        {
            ident.transform.position = position;
        }

        /// <summary>
        /// Draw spawn region.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, _spawnRegion);
        }
        #endregion
    }


}