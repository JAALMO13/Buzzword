using FirstGearGames.LobbyAndWorld.Clients;
using FishNet;
using FishNet.Object;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases
{


    public class ReadyMemberEntry : MemberEntry
    {
        #region Serialized.
        /// <summary>
        /// Icon to show if member is currently ready.
        /// </summary>
        [Tooltip("Icon to show if member is currently ready.")]
        [SerializeField]
        private Image _readyIcon = null;
        /// <summary>
        /// Ready button image to change color on.
        /// </summary>
        [Tooltip("Ready button image to change color on.")]
        [SerializeField]
        private Image _readyButtonImage = null;
        /// <summary>
        /// Color to show when ready.
        /// </summary>
        [Tooltip("Color to show when ready.")]
        [SerializeField]
        private Color _readyColor = Color.green;
        /// <summary>
        /// Color to show when not ready.
        /// </summary>
        [Tooltip("Color to show when not ready.")]
        [SerializeField]
        private Color _unreadyColor = Color.white;
        #endregion

        #region Private.
        /// <summary>
        /// Current ready state of this object.
        /// </summary>
        private bool _ready = false;
        /// <summary>
        /// True if awaiting for a ready response.
        /// </summary>
        private bool _awaitingReadyResponse = false;
        #endregion

        public override void FirstInitialize(CurrentRoomMenu crm, NetworkObject id, bool started)
        {
            base.FirstInitialize(crm, id, started);
            /* Ready ready button visibile based on if this is local client entry.
             * Host however doesn't need a ready button. They will automatically
             * set ready since they decide when to start the game. */

            NetworkObject localNob = InstanceFinder.ClientManager.Connection.FirstObject;

            bool local = (id == localNob);
            //If for local player.
            if (local)
            {
                /* Multiple conditions will set the player as automatically
                 * ready.
                 * Being the host of the room.
                 * Or if the match has already started. */
                if (LobbyNetwork.CurrentRoom.IsStarted || LobbyNetwork.IsRoomHost(LobbyNetwork.CurrentRoom, localNob))
                {
                    _readyButtonImage.gameObject.SetActive(false);
                    _readyIcon.gameObject.SetActive(true);
                    SetLocalReady(true);
                }
                //Otherwise show ready button.
                else
                {
                    _readyButtonImage.gameObject.SetActive(true);
                    _readyIcon.gameObject.SetActive(false);

                }
            }
            //Not for local player.
            else
            {
                _readyButtonImage.gameObject.SetActive(false);
                _readyIcon.gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            ReadyLobbyNetwork.OnMemberSetReady += ReadyLobbyNetwork_OnMemberSetReady;
        }

        private void OnDestroy()
        {
            ReadyLobbyNetwork.OnMemberSetReady -= ReadyLobbyNetwork_OnMemberSetReady;
        }

        /// <summary>
        /// Called when a member changes their ready state.
        /// </summary>
        private void ReadyLobbyNetwork_OnMemberSetReady(NetworkObject arg1, bool ready)
        {
            //Not for this member.
            if (arg1 != base.MemberId)
                return;

            //For local client.
            if (base.MemberId == InstanceFinder.ClientManager.Connection.FirstObject)
            {
                _awaitingReadyResponse = false;
                Color c = (ready) ? _readyColor : _unreadyColor;
                _readyButtonImage.color = c;
                _ready = ready;
            }
            //For another player.
            else
            {
                _readyIcon.gameObject.SetActive(ready);
            }
        }

        /// <summary>
        /// Called when ready is pressed.
        /// </summary>
        public void OnClick_Ready()
        {
            SetLocalReady(!_ready);
        }

        /// <summary>
        /// Changes the locla state of ready.
        /// </summary>
        /// <param name="ready"></param>
        private void SetLocalReady(bool ready)
        {
            //Don't do anything if not for local client. Only local client should be able to click this anyway.
            if (base.MemberId != InstanceFinder.ClientManager.Connection.FirstObject)
                return;
            if (_awaitingReadyResponse)
                return;

            _awaitingReadyResponse = true;
            ReadyLobbyNetwork.SetReady(!_ready);
        }

    }


}