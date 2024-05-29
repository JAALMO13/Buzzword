
using FishNet.Object;

using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases
{


    public class RoomDetails
    {
        public RoomDetails() { }
        public RoomDetails(string name, string password, int _min, int _max, int _time, int maxPlayers=2)
        {
            Name = name;
            Password = password;
            min = _min;
            max = _max;
            time = _time;
            IsPassworded = !string.IsNullOrEmpty(password);
            MaxPlayers=maxPlayers;
        }

        /// <summary>
        /// Name of this room.
        /// </summary>
        public string Name;
        /// <summary>
        /// Maximum players which may join this room.
        /// </summary>
        public int MaxPlayers;
        /// <summary>
        /// True if this room has started.
        /// </summary>
        public bool IsStarted;
        public int min;
        public int max;
        public int time;
        /// <summary>
        /// True if this room requires a password.
        /// </summary>
        public bool IsPassworded;
        /// <summary>
        /// Password for this room. This will only exist on the server.
        /// </summary>
        [System.NonSerialized]
        public string Password = string.Empty;
        /// <summary>
        /// True to lock the room from new joiners after the game starts.
        /// </summary>
        public bool LockOnStart;
        /// <summary>
        /// Scenes loaded for this room. Only available on server.
        /// </summary>
        [System.NonSerialized]
        public HashSet<Scene> Scenes = new HashSet<Scene>();
        /// <summary>
        /// Members in this room. Using an array so that it can serialize over Mirror.
        /// </summary>
        public List<NetworkObject> MemberIds = new List<NetworkObject>();
        /// <summary>
        /// Members which have started the game, and are currently in the game scene.
        /// </summary>
        // [System.NonSerialized]
        public List<NetworkObject> StartedMembers = new List<NetworkObject>();
        /// <summary>
        /// Members kicked from this room. Only stored on the server.
        /// </summary>
        [System.NonSerialized]
        public List<NetworkObject> KickedIds = new List<NetworkObject>();

        /// <summary>
        /// Adds to Members.
        /// </summary>
        /// <param name="clientId"></param>
        internal void AddMember(NetworkObject clientId)
        {
            if (!MemberIds.Contains(clientId))
                MemberIds.Add(clientId);
        }

        /// <summary>
        /// Adds a member to StartedMembers.
        /// </summary>
        /// <param name="clientId"></param>
        internal void AddStartedMember(NetworkObject clientId)
        {
            if (!StartedMembers.Contains(clientId))
                StartedMembers.Add(clientId);
        }
        /// <summary>
        /// Removes from Members.
        /// </summary>
        /// <param name="clientId"></param>
        internal bool RemoveMember(NetworkObject clientId)
        {
            int index = MemberIds.IndexOf(clientId);
            if (index != -1)
            {
                MemberIds.RemoveAt(index);
                //Also try to remove from started.
                StartedMembers.Remove(clientId);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Clears MemberIds.
        /// </summary>
        internal void ClearMembers()
        {
            MemberIds.Clear();
        }

        /// <summary>
        /// Returns if a client is kicked from this room.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        internal bool IsKickedMember(NetworkObject clientId)
        {
            return KickedIds.Contains(clientId);
        }

        /// <summary>
        /// Adds to kicked.
        /// </summary>
        /// <param name="clientId"></param>
        internal void AddKicked(NetworkObject clientId)
        {
            //Already in kicked.
            if (IsKickedMember(clientId))
                return;

            KickedIds.Add(clientId);
        }
    }


}