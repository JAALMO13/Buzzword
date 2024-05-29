using FirstGearGames.LobbyAndWorld.Clients;
using FirstGearGames.LobbyAndWorld.Extensions;
using FirstGearGames.LobbyAndWorld.Global;
using FirstGearGames.LobbyAndWorld.Global.Canvases;
using FishNet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases
{


    public class CurrentRoomMenu : MonoBehaviour
    {
        #region Types.
        /// <summary>
        /// Ways to process a room status response.
        /// </summary>
        private enum RoomProcessingTypes
        {
            Unset,
            Create,
            Join,
            Leave,
            Start
        }

        #endregion

        #region Public.
        /// <summary>
        /// Current member entries.
        /// </summary>
        [HideInInspector]
        public List<MemberEntry> MemberEntries = new List<MemberEntry>();
        #endregion

        #region Serialized.
        /// <summary>
        /// Button used to start the game.
        /// </summary>
        [Tooltip("Button used to start the game.")]
        [SerializeField]
        private Button _startButton;
        /// <summary>
        /// Content to hold room member listings.
        /// </summary>
        [Tooltip("Content to hold room member listings.")]
        [SerializeField]
        private Transform _membersContent;
        /// <summary>
        /// Prefab to spawn for each member entry.
        /// </summary>
        [Tooltip("Prefab to spawn for each member entry.")]
        [SerializeField]
        private MemberEntry _memberEntryPrefab;
        #endregion

        #region Private.
        /// <summary>
        /// CanvasGroup on this object.
        /// </summary>
        private CanvasGroup _canvasGroup;
        /// <summary>
        /// True when waiting for start response.
        /// </summary>
        private bool _awaitingStartResponse = false;
        /// <summary>
        /// True when waiting for kick response.
        /// </summary>
        private bool _awaitingkickResponse = false;
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
            _awaitingkickResponse = false;
            _awaitingStartResponse = false;

            //Destroy children of content. This is just to get rid of any placeholders.
            foreach (Transform c in _membersContent)
                Destroy(c.gameObject);
            MemberEntries.Clear();
            ProcessRoomStatus(RoomProcessingTypes.Unset, false, null, string.Empty);
        }

        /// <summary>
        /// Shows canvases for a successful room creation.
        /// </summary>
        /// <param name="show"></param>
        public void ShowRoomCreatedSuccess(RoomDetails roomDetails)
        {
            ProcessRoomStatus(RoomProcessingTypes.Create, true, roomDetails, string.Empty);
        }
        /// <summary>
        /// Shows canvases for a failed room creation.
        /// </summary>
        /// <param name="show"></param>
        public void ShowRoomCreatedFailed(string failedReason)
        {
            ProcessRoomStatus(RoomProcessingTypes.Create, false, null, failedReason);
        }


        /// <summary>
        /// Called when successfully joined a room.
        /// </summary>
        /// <param name="roomDetails"></param>
        public void ShowRoomJoinedSuccess(RoomDetails roomDetails)
        {
            ProcessRoomStatus(RoomProcessingTypes.Join, true, roomDetails, string.Empty);
        }
        /// <summary>
        /// Called when failed to join a room.
        /// </summary>
        /// <param name="failedReason"></param>
        public void ShowRoomJoinedFailed(string failedReason)
        {
            ProcessRoomStatus(RoomProcessingTypes.Join, false, null, failedReason);
        }

        /// <summary>
        /// Called when successfully leaving a room.
        /// </summary>
        public void ShowRoomLeftSuccess()
        {
            ProcessRoomStatus(RoomProcessingTypes.Leave, true, null, string.Empty);
        }

        /// <summary>
        /// Called when failing to leave a room.
        /// </summary>
        public void ShowRoomLeftFailed()
        {
            ProcessRoomStatus(RoomProcessingTypes.Leave, false, null, string.Empty);
        }

        /// <summary>
        /// Shows canvases based on start game status.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failedReason"></param>
        public void ShowStartGame(bool success, RoomDetails roomDetails, string failedReason)
        {
            _awaitingStartResponse = false;
            ProcessRoomStatus(RoomProcessingTypes.Start, success, roomDetails, failedReason);
        }

        /// <summary>
        /// Processes the results of room actions.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="roomDetails"></param>
        /// <param name="failedReason"></param>
        private void ProcessRoomStatus(RoomProcessingTypes processType, bool success, RoomDetails roomDetails, string failedReason)
        {
            bool hideCondition = (processType == RoomProcessingTypes.Unset) ||
                (processType == RoomProcessingTypes.Leave && success) ||
                (processType == RoomProcessingTypes.Start && success) ||
                (processType == RoomProcessingTypes.Join && !success) ||
                (processType == RoomProcessingTypes.Create && !success);
            bool show = !hideCondition;

            //Set active based on success.
            _canvasGroup.SetActive(show, true);
            //If hiding also destroy entries.
            if (!show)
                DestroyMemberEntries();

            /* StartButton may become interactable for a number of reasons.
            * Such as, being the host, host leaving, or joining an already
            * started game. */
            UpdateStartButton();

            /* Don't update room actions canvas when a room starts.
             * This is the global canvas which allows players to
             * leave the room. Players should still be allowed to leave
             * after room starts. */
            bool updateRoomActions = (processType != RoomProcessingTypes.Start);
            if (updateRoomActions)
            {
                string roomName = (roomDetails == null) ? string.Empty : roomDetails.Name;
                GlobalManager.CanvasesManager.RoomActionCanvas.ShowCurrentRoom((success && show), roomName);
            }

            //If failed to create room.
            if (failedReason != string.Empty)
                GlobalManager.CanvasesManager.MessagesCanvas.InfoMessages.ShowTimedMessage(failedReason, Color.red);
        }

        /// <summary>
        /// Destroys current member entries.
        /// </summary>
        private void DestroyMemberEntries()
        {
            for (int i = 0; i < MemberEntries.Count; i++)
                Destroy(MemberEntries[i].gameObject);

            MemberEntries.Clear();
        }

        /// <summary>
        /// Creates member entries for all members in roomDetails.
        /// </summary>
        /// <param name="roomDetails"></param>
        private void CreateMemberEntries(RoomDetails roomDetails)
        {
            DestroyMemberEntries();
            UpdateStartButton();

            bool host = LobbyNetwork.IsRoomHost(roomDetails, InstanceFinder.ClientManager.Connection.FirstObject);
            //Add current members to content.
            for (int i = 0; i < roomDetails.MemberIds.Count; i++)
            {
                MemberEntry entry = Instantiate(_memberEntryPrefab, _membersContent);
                entry.FirstInitialize(this, roomDetails.MemberIds[i], roomDetails.StartedMembers.Contains(roomDetails.MemberIds[i]));
                /* Set kick active if member isnt self, match hasnt already started,
                 * and if host. */
                entry.SetKickActive(
                    roomDetails.MemberIds[i] != InstanceFinder.ClientManager.Connection.FirstObject &&
                    host &&
                    !roomDetails.IsStarted
                    );

                MemberEntries.Add(entry);
            }
        }

        /// <summary>
        /// Updates if start button is enabled.
        /// </summary>
        private void UpdateStartButton()
        {
            string startFailedString = string.Empty;
            _startButton.interactable = LobbyNetwork.CanUseStartButton(LobbyNetwork.CurrentRoom, InstanceFinder.ClientManager.Connection.FirstObject);
        }

        /// <summary>
        /// Updates the rooms list.
        /// </summary>
        /// <param name="roomDetails"></param>
        public void UpdateRoom(RoomDetails[] roomDetails)
        {
            RoomDetails currentRoom = LobbyNetwork.CurrentRoom;
            //Not in a room, nothing to update.
            if (currentRoom == null)
                return;

            for (int i = 0; i < roomDetails.Length; i++)
            {
                //Not current room.
                if (roomDetails[i].Name != currentRoom.Name)
                    continue;

                /* It's easier to just re-add entries so 
                 * that's what we'll do. */
                CreateMemberEntries(roomDetails[i]);
            }

        }

        /// <summary>
        /// Received when Start Game is pressed.
        /// </summary>
        public void OnClick_StartGame()
        {
            //Still waiting for a server response.
            if (_awaitingStartResponse)
                return;

            string failedReason = string.Empty;
            bool result = LobbyNetwork.CanStartRoom(LobbyNetwork.CurrentRoom, InstanceFinder.ClientManager.Connection.FirstObject, ref failedReason, false);
            if (!result)
            {
                GlobalManager.CanvasesManager.MessagesCanvas.InfoMessages.ShowTimedMessage(failedReason, MessagesCanvas.BRIGHT_ORANGE);
            }
            else
            {
                _awaitingStartResponse = true;
                _startButton.interactable = false;
                LobbyNetwork.StartGame();
            }
        }

        /// <summary>
        /// Tries to kick a member.
        /// </summary>
        /// <param name="entry"></param>
        public void KickMember(MemberEntry entry)
        {
            if (_awaitingkickResponse)
                return;
            if (entry.MemberId == null)
                return;

            _awaitingkickResponse = true;
            //Try to kick member Id on entry.
            LobbyNetwork.KickMember(entry.MemberId);
        }

        /// <summary>
        /// Received after successfully kicking a member.
        /// </summary>
        public void ProcessKickMemberSuccess()
        {
            _awaitingkickResponse = false;
        }
        /// <summary>
        /// Received after failing to kick a member.
        /// </summary>
        /// <param name="failedReason"></param>
        public void ProcessKickMemberFailed(string failedReason)
        {
            _awaitingkickResponse = false;
            if (failedReason != string.Empty)
                GlobalManager.CanvasesManager.MessagesCanvas.InfoMessages.ShowTimedMessage(failedReason, Color.red);
        }

    }


}