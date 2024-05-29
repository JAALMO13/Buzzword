using FirstGearGames.LobbyAndWorld.Extensions;
using FirstGearGames.LobbyAndWorld.Global;
using FirstGearGames.LobbyAndWorld.Global.Canvases;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases
{

    public class JoinRoomMenu : MonoBehaviour
    {
        #region Serialized.       
        /// <summary>
        /// Transform containing rooms list.
        /// </summary>
        [Tooltip("Transform containing rooms list.")]
        [SerializeField]
        private Transform _roomsContent;
        /// <summary>
        /// Prefab for room entries.
        /// </summary>
        [Tooltip("Prefab for room entries.")]
        [SerializeField]
        private RoomEntry _entryPrefab;
        /// <summary>
        /// Canvas to show if entering a password is required.
        /// </summary>
        [Tooltip("Canvas to show if entering a password is required.")]
        [SerializeField]
        private CanvasGroup _passwordCanvas;
        /// <summary>
        /// Text holding the entered room password.
        /// </summary>
        [Tooltip("Text holding the entered room password.")]
        [SerializeField]
        private TMP_InputField _passwordText;
        #endregion

        #region Private.
        /// <summary>
        /// CanvasGroup on this object.
        /// </summary>
        private CanvasGroup _canvasGroup;
        /// <summary>
        /// Current available rooms.
        /// </summary>
        private List<RoomEntry> _roomEntries = new List<RoomEntry>();
        /// <summary>
        /// True if waiting for server to respond to password entry.
        /// </summary>
        private bool _awaitingPasswordResponse = false;
        /// <summary>
        /// Room to try and join after clicking join and or entering a password.
        /// </summary>
        private string _cachedRoomName = string.Empty;
        #endregion

        /// <summary>
        /// Initialize this script for use. Should only be completed once.
        /// </summary>
        /// <param name="joinCreateRoomCanvas"></param>
        public void FirstInitialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Reset();
        }

        /// <summary>
        /// Resets canvases as though first login.
        /// </summary>
        public void Reset()
        {
            ShowRoomJoinedFailed();
            _passwordCanvas.SetActive(false, true);
            //Destroy current room entries.
            for (int i = 0; i < _roomEntries.Count; i++)
                Destroy(_roomEntries[i].gameObject);
            _roomEntries.Clear();
        }

        /// <summary>
        /// Shows canvases for a successful room creation.
        /// </summary>
        /// <param name="show"></param>
        public void ShowRoomCreatedSuccess()
        {
            _canvasGroup.SetActive(false, true);
        }
    
        /// <summary>
        /// Shows when successully joined a room.
        /// </summary>
        public void ShowRoomJoinedSuccess()
        {
            _awaitingPasswordResponse = false;
            _passwordCanvas.SetActive(false, true);
            _canvasGroup.SetActive(false, true);
        }
        /// <summary>
        /// Shows when failed to joined a room.
        /// </summary>
        public void ShowRoomJoinedFailed()
        {
            _awaitingPasswordResponse = false;
            _canvasGroup.SetActive(true, true);
        }


        /// <summary>
        /// Called when successfully leaving a room.
        /// </summary>        
        public void ShowRoomLeftSuccess()
        {
            _canvasGroup.SetActive(true, true);
        }
        /// <summary>
        /// Called when failing to leaving a room.
        /// </summary>        
        public void ShowRoomLeftFailed()
        {
            _canvasGroup.SetActive(false, true);
        }


        /// <summary>
        /// Updates the rooms list.
        /// </summary>
        /// <param name="roomDetails"></param>
        public void UpdateRooms(RoomDetails[] roomDetails)
        {
            for (int i = 0; i < roomDetails.Length; i++)
            {
                int index = ReturnRoomEntriesIndex(roomDetails[i].Name);
                //If not found.
                if (index == -1)
                {
                    /* No members,
                     * At maximum members,
                     * or Started + LockOnStart. */
                    if (roomDetails[i].MemberIds.Count == 0 ||
                        roomDetails[i].MemberIds.Count >= roomDetails[i].MaxPlayers ||
                        (roomDetails[i].IsStarted && roomDetails[i].LockOnStart))
                        continue;

                    //Not found but has members, need to make a new entry.
                    RoomEntry entry = Instantiate(_entryPrefab, _roomsContent);
                    entry.Initialize(this, roomDetails[i]);
                    _roomEntries.Add(entry);
                }
                //Found.
                else
                {
                    /* Immediately re-initialize with current values.
                     * This could be extra work as sometimes entrys will
                     * be destroyed later in this code. But this makes checks
                     * easier and more reliable. */
                    _roomEntries[index].Initialize(this, roomDetails[i]);

                    //If no members or room has changed to started then remove entry.
                    if (_roomEntries[index].RoomDetails.MemberIds.Count == 0)
                    {
                        Destroy(_roomEntries[index].gameObject);
                        _roomEntries.RemoveAt(index);
                    }
                    //Members updated.
                    else
                    {
                        if (_roomEntries[index].RoomDetails.MemberIds.Count >= _roomEntries[index].RoomDetails.MaxPlayers ||
                            (_roomEntries[index].RoomDetails.IsStarted && _roomEntries[index].RoomDetails.LockOnStart))
                        {
                            Destroy(_roomEntries[index].gameObject);
                            _roomEntries.RemoveAt(index);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Returns an index of the specified room name.
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        private int ReturnRoomEntriesIndex(string roomName)
        {
            //Use a for loop instead of linq to avoid allocations.
            for (int i = 0; i < _roomEntries.Count; i++)
            {
                if (_roomEntries[i].RoomDetails.Name == roomName)
                    return i;
            }

            //Fall through, not found.
            return -1;
        }

        /// <summary>
        /// Tries to join a room.
        /// </summary>
        /// <param name="roomName"></param>
        public void JoinRoom(RoomEntry roomEntry)
        {
            _cachedRoomName = roomEntry.RoomDetails.Name;
            if (roomEntry.RoomDetails.IsPassworded)
            {
                _passwordText.text = string.Empty;
                _passwordCanvas.SetActive(true, true);
            }
            else
            { 
                LobbyNetwork.JoinRoom(_cachedRoomName, string.Empty);
            }
        }

        /// <summary>
        /// Called when cancel is pressed on the password canvas.
        /// </summary>
        public void OnClick_CancelPassword()
        {
            _passwordCanvas.SetActive(false, true);
        }

        /// <summary>
        /// Called when the join button is pressed on the password canvas.
        /// </summary>
        public void OnClick_JoinPassword()
        {
            if (_awaitingPasswordResponse)
                return;

            string password = _passwordText.text;
            LobbyNetwork.SanitizeTextMeshProString(ref password);
            if (string.IsNullOrEmpty(password))
            {
                GlobalManager.CanvasesManager.MessagesCanvas.InfoMessages.ShowTimedMessage("A password is required to join this room.", MessagesCanvas.BRIGHT_ORANGE);
            }
            //Try to join with supplied password.
            else
            {
                LobbyNetwork.JoinRoom(_cachedRoomName, password);
            }
        }

    }


}