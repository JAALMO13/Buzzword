using FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases;
using FirstGearGames.LobbyAndWorld.Lobbies.SignInCanvases;
using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Lobbies
{

    public class LobbyCanvases : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("SignInCanvas reference.")]
        [SerializeField]
        private SignInCanvas _signInCanvas;
        /// <summary>
        /// SignInCanvas reference.
        /// </summary>
        public SignInCanvas SignInCanvas { get { return _signInCanvas; } }
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("JoinCreateRoomCanvas reference.")]
        [SerializeField]
        private JoinCreateRoomCanvas _joinCreateRoomCanvas;
        /// <summary>
        /// JoinCreateRoomCanvas reference.
        /// </summary>
        public JoinCreateRoomCanvas JoinCreateRoomCanvas { get { return _joinCreateRoomCanvas; } }
        /// <summary>
        /// Camera for the lobby.
        /// </summary>
        [Tooltip("Camera for the lobby.")]
        [SerializeField]
        private Camera _lobbyCamera = null;



        /// <summary>
        /// Initializes this script for use. Should only be completed once.
        /// </summary>
        /// <param name="lobbyNetwork"></param>
        public void FirstInitialize()
        {
            SignInCanvas.FirstInitialize(this);
            JoinCreateRoomCanvas.FirstInitialize(this);
        }

        /// <summary>
        /// Resets these canvases/menus as if first being used.
        /// </summary>
        public void Reset()
        {
            SignInCanvas.Reset();
            JoinCreateRoomCanvas.Reset();
        }

        /// <summary>
        /// Sets LobbyCamera active state.
        /// </summary>
        /// <param name="active"></param>
        public void SetLobbyCameraActive(bool active)
        {
            if (_lobbyCamera != null)
                _lobbyCamera.gameObject.SetActive(active);
        }
    }


}