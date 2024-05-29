using FirstGearGames.LobbyAndWorld.Extensions;
using FirstGearGames.LobbyAndWorld.Lobbies;
using TMPro;
using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Global.Canvases
{


    public class RoomActionsCanvas : MonoBehaviour
    {
        #region Serialized.
        /// <summary>
        /// Text to show current room.
        /// </summary>
        [Tooltip("Text to show current room.")]
        [SerializeField]
        private TextMeshProUGUI _currentRoomText;
        #endregion

        #region Private.
        /// <summary>
        /// CanvasGroup on this object.
        /// </summary>
        private CanvasGroup _canvasGroup;
        #endregion

        #region Const.
        /// <summary>
        /// Prefix for signedInText.
        /// </summary>
        private const string CURRENT_ROOM_PREFIX = "Currently in ";
        #endregion

        private void Awake()
        {
            FirstInitialize();
        }

        /// <summary>
        /// Initializes this script for use. Should only be completed once.
        /// </summary>
        private void FirstInitialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Reset();
        }

        /// <summary>
        /// Resets canvases as though first login.
        /// </summary>
        public void Reset()
        {
            ShowCurrentRoom(false);
        }

        /// <summary>
        /// Sets signed in username text.
        /// </summary>
        /// <param name="username"></param>
        private void SetCurrentRoom(string username)
        {
            _currentRoomText.text = CURRENT_ROOM_PREFIX + username;
        }

        /// <summary>
        /// Clears signedInText.
        /// </summary>
        private void ClearCurrentRoom()
        {
            _currentRoomText.text = string.Empty;
        }

        /// <summary>
        /// Shows canvases based on signedIn status.
        /// </summary>
        /// <param name="inRoom"></param>
        public void ShowCurrentRoom(bool inRoom, string roomName = "")
        {
            _canvasGroup.SetActive(inRoom, true);

            if (inRoom)
                SetCurrentRoom(roomName);
            else
                ClearCurrentRoom();
        }

        /// <summary>
        /// Received when Leave is pressed.
        /// </summary>
        public void OnClick_Leave()
        {
            //Hide current room.
            ShowCurrentRoom(false);
            LobbyNetwork.LeaveRoom();
        }

    }

}
