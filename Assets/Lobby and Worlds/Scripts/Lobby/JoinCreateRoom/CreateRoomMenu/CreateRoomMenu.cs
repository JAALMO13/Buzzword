using FirstGearGames.LobbyAndWorld.Extensions;
using FirstGearGames.LobbyAndWorld.Global;
using FirstGearGames.LobbyAndWorld.Global.Canvases;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

namespace FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases
{


    public class CreateRoomMenu : MonoBehaviour
    {
        #region Serialized.
        /// <summary>
        /// Button to create a room.
        /// </summary>
        [Tooltip("Button to create a room.")]
        [SerializeField]
        private Button _createRoomButton;
        /// <summary>
        /// Text holding the room name.
        /// </summary>
        [Tooltip("Text holding the room name.")]
        [SerializeField]
        private TMP_InputField _nameText;
        /// <summary>
        /// Text holding the room password.
        /// </summary>
        [Tooltip("Text holding the room password.")]
        [SerializeField]
        private TMP_InputField _passwordText;
        /// <summary>
        /// Lock On Start toggle.
        /// </summary>
        [Tooltip("Lock On Start toggle.")]
        [SerializeField]
        private Toggle _lockOnStart;
        /// <summary>
        /// Dropdown holding player count.
        /// </summary>
        [Tooltip("Dropdown holding player count.")]
        [SerializeField]
        private TMP_Dropdown _gameModes;
        #endregion

        #region Private.
        /// <summary>
        /// CanvasGroup on this object.
        /// </summary>
        private CanvasGroup _canvasGroup;
        /// <summary>
        /// True if awaiting a create response from the server.
        /// </summary>
        private bool _awaitingCreateResponse = false;
        #endregion

        /// <summary>
        /// Initialize this script for use. Should only be completed once.
        /// </summary>
        /// <param name="joinCreateRoomCanvas"></param>
        public void FirstInitialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            //Rebuild player count.
            // _gameModes.ClearOptions();
            // List<string> options = new List<string>();
            // int minimum = LobbyNetwork.ReturnMinimumPlayers();
            // int maximum = LobbyNetwork.ReturnMaximumPlayers();
            // for (int i = minimum; i < maximum; i++)
            //     options.Add(i.ToString());

            // _gameModes.AddOptions(options);
            Reset();
        }

        /// <summary>
        /// Resets canvases as though first login.
        /// </summary>
        public void Reset()
        {
            _awaitingCreateResponse = false;
            ShowRoomCreatedFailed();
        }

        /// <summary>
        /// Called when create room is clicked.
        /// </summary>
        // * when play is clicked in either game settings views
        public void OnClick_CreateRoom()
        {
            if (_awaitingCreateResponse)
                return;

            string roomName = _nameText.text.Trim();
            string password = _passwordText.text;
            // int playerCount = ConvertPlayerCount(_gameModes.captionText.text);
            int playerCount = 2;

            string failedReason = string.Empty;
            //If cannot create,
            if (!LobbyNetwork.SanitizeRoomName(ref roomName, ref failedReason) || !LobbyNetwork.SanitizePlayerCount(playerCount, ref failedReason))
            {
                GlobalManager.CanvasesManager.MessagesCanvas.InfoMessages.ShowTimedMessage(failedReason, MessagesCanvas.BRIGHT_ORANGE);
            }
            //Can try to create.
            else
            {
                _awaitingCreateResponse = true;
                LobbyNetwork.SanitizeTextMeshProString(ref password);
                // LobbyNetwork.CreateRoom(roomName, password, );
            }

            StartCoroutine(WaitForScene());
        }
        /// <summary>
        /// Sanitizes player count string.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>        
        private int ConvertPlayerCount(string countText)
        {
            int result;
            if (!int.TryParse(countText, out result))
                return 0;
            else
                return result;
        }


        /// <summary>
        /// Shows canvases for a successful room creation.
        /// </summary>
        public void ShowRoomCreatedSuccess()
        {
            _passwordText.text = string.Empty;
            _awaitingCreateResponse = false;
            _canvasGroup.SetActive(false, true);
        }
        /// <summary>
        /// Shows canvases for a failed room creation.
        /// </summary>
        public void ShowRoomCreatedFailed()
        {
            _awaitingCreateResponse = false;
            _canvasGroup.SetActive(true, true);
        }


        /// <summary>
        /// Called when successfully joined a room.
        /// </summary>
        public void ShowRoomJoinedSuccess()
        {
            _canvasGroup.SetActive(false, true);
        }
        /// <summary>
        /// Called when failed to joined a room.
        /// </summary>
        public void ShowRoomJoinedFailed()
        {
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
        /// Called when failing to leave a room.
        /// </summary>
        public void ShowRoomLeftFailed()
        {
            _canvasGroup.SetActive(false, true);
        }

        IEnumerator WaitForScene()
        {
            yield return new WaitUntil(() => SceneManager.sceneCount > 1);
            yield return new WaitUntil(() => SceneManager.GetSceneByName("MainGame").isLoaded);

            GameManager.Instance.gameMode = _gameModes.captionText.text;
        }
    }


}