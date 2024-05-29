using FirstGearGames.LobbyAndWorld.Global;
using UnityEngine;


namespace FirstGearGames.LobbyAndWorld.Lobbies.SignInCanvases
{

    public class SignInCanvas : MonoBehaviour
    {
        /// <summary>
        /// LobbyCanvases reference.
        /// </summary>
        public LobbyCanvases LobbyCanvases { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("SignInMenu reference.")]
        [SerializeField]
        private SignInMenu _signInMenu;
        /// <summary>
        /// SignInMenu reference.
        /// </summary>
        public SignInMenu SignInMenu { get { return _signInMenu; } }


        /// <summary>
        /// Initializes this script for use. Should only be completed once.
        /// </summary>
        /// <param name="lobbyCanvases"></param>
        public void FirstInitialize(LobbyCanvases lobbyCanvases)
        {
            LobbyCanvases = lobbyCanvases;
            SignInMenu.FirstInitialize(this);
            Reset();
        }

        /// <summary>
        /// Resets these canvases/menus as if first being used.
        /// </summary>
        public void Reset()
        {
            SignInMenu.Reset();
            GlobalManager.CanvasesManager.UserActionsCanvas.Reset();
        }

        /// <summary>
        /// Shows canvases for a successful sign in.
        /// </summary>
        /// <param name="signedIn"></param>
        public void SignInSuccess(string username)
        {
            SignInMenu.SignInSuccess();
            GlobalManager.CanvasesManager.UserActionsCanvas.SignInSuccess(username);
        }

        /// <summary>
        /// Shows canvases for a failed sign in.
        /// </summary>
        /// <param name="signedIn"></param>
        public void SignInFailed(string failedReason)
        {
            SignInMenu.SignInFailed(failedReason);
            GlobalManager.CanvasesManager.UserActionsCanvas.SignInFailed();
        }

    }

}