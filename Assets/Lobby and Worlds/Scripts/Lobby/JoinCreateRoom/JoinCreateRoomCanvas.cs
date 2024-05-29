using FirstGearGames.LobbyAndWorld.Extensions;
using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases
{


    public class JoinCreateRoomCanvas : MonoBehaviour
    {
        #region Public.
        /// <summary>
        /// LobbyCanvases reference.
        /// </summary>
        public LobbyCanvases LobbyCanvases { get; private set; }
        #endregion

        #region Serialized.
        /// <summary>
        /// CreateRoomMenu reference.
        /// </summary>
        [Tooltip("CreateRoomMenu reference.")]
        [SerializeField]
        private CreateRoomMenu _createRoomMenu;
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("JoinRoomMenu reference.")]
        [SerializeField]
        private JoinRoomMenu _joinRoomMenu;
        /// <summary>
        /// JoinRoomMenu reference.
        /// </summary>
        public JoinRoomMenu JoinRoomMenu { get { return _joinRoomMenu; } }
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("CurrentRoomMenu reference.")]
        [SerializeField]
        private CurrentRoomMenu _currentRoomMenu;
        /// <summary>
        /// CurrentRoomMenu reference.
        /// </summary>
        public CurrentRoomMenu CurrentRoomMenu { get { return _currentRoomMenu; } }
        #endregion

        #region Private.
        /// <summary>
        /// CanvasGroup on this object.
        /// </summary>
        private CanvasGroup _canvasGroup;
        #endregion

        /// <summary>
        /// Initializes this script for use. Should only be completed once.
        /// </summary>
        /// <param name="lobbyCanvases"></param>
        public void FirstInitialize(LobbyCanvases lobbyCanvases)
        {
            LobbyCanvases = lobbyCanvases;
            _canvasGroup = GetComponent<CanvasGroup>();
            _createRoomMenu.FirstInitialize();
            JoinRoomMenu.FirstInitialize();
            CurrentRoomMenu.FirstInitialize();
            Reset();
        }

        /// <summary>
        /// Resets these canvases/menus as if first being used.
        /// </summary>
        public void Reset()
        {
            SignInFailed();
            _createRoomMenu.Reset();
            JoinRoomMenu.Reset();
            CurrentRoomMenu.Reset();
        }

        /// <summary>
        /// Called when succesfully signing in.
        /// </summary>
        public void SignInSuccess()
        {
            _canvasGroup.SetActive(true, true);
        }
        /// <summary>
        /// Called when failing to sign in.
        /// </summary>
        public void SignInFailed()
        {
            _canvasGroup.SetActive(false, true);
        }

        /// <summary>
        /// Shows canvases for a successful room creation.
        /// </summary>
        /// <param name="show"></param>
        public void ShowRoomCreatedSuccess(RoomDetails roomDetails)
        {
            JoinRoomMenu.ShowRoomCreatedSuccess();
            _createRoomMenu.ShowRoomCreatedSuccess();
            CurrentRoomMenu.ShowRoomCreatedSuccess(roomDetails);
        }
        /// <summary>
        /// Shows canvases for a failed room creation.
        /// </summary>
        /// <param name="show"></param>
        public void ShowRoomCreatedFailed(string failedReason)
        {
            _createRoomMenu.ShowRoomCreatedFailed();
            CurrentRoomMenu.ShowRoomCreatedFailed(failedReason);
        }


        /// <summary>
        /// Called when successfully joined a room.
        /// </summary>
        /// <param name="show"></param>
        public void ShowRoomJoinedSuccess(RoomDetails roomDetails)
        {
            JoinRoomMenu.ShowRoomJoinedSuccess();
            _createRoomMenu.ShowRoomJoinedSuccess();
            CurrentRoomMenu.ShowRoomJoinedSuccess(roomDetails);
        }
        /// <summary>
        /// Called when failed to join a room.
        /// </summary>
        /// <param name="show"></param>
        public void ShowRoomJoinedFailed(string failedReason)
        {
            JoinRoomMenu.ShowRoomJoinedFailed();
            _createRoomMenu.ShowRoomJoinedFailed();
            CurrentRoomMenu.ShowRoomJoinedFailed(failedReason);
        }

        /// <summary>
        /// Called when successfully leaving a room.
        /// </summary>
        public void ShowRoomLeftSuccess()
        {
            JoinRoomMenu.ShowRoomLeftSuccess();
            _createRoomMenu.ShowRoomLeftSuccess();
            CurrentRoomMenu.ShowRoomLeftSuccess();
        }
        /// <summary>
        /// Called when failing to leaving a room.
        /// </summary>
        public void ShowRoomLeftFailed()
        {
            JoinRoomMenu.ShowRoomLeftFailed();
            _createRoomMenu.ShowRoomLeftFailed();
            CurrentRoomMenu.ShowRoomLeftFailed();
        }

        /// <summary>
        /// Shows canvases based on room left status.
        /// </summary>
        public void ShowStartGame(bool success, RoomDetails roomDetails, string failedReason)
        {
            /* StartGame response won't affect
             * JoinRoonMenu or CreateRoomMenu as they
             * both are already hidden because of being 
             * in a CurrentRoom. Only current room must update. */
            CurrentRoomMenu.ShowStartGame(success, roomDetails, failedReason);
        }

    }


}