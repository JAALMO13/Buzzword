using FirstGearGames.LobbyAndWorld.Extensions;
using FirstGearGames.LobbyAndWorld.Global;
using FirstGearGames.LobbyAndWorld.Global.Canvases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using FishNet;
namespace FirstGearGames.LobbyAndWorld.Lobbies.SignInCanvases
{

    public class SignInMenu : MonoBehaviour
    {
        /// <summary>
        /// Input field to disable when trying to login.
        /// </summary>
        [Tooltip("Input field to disable when trying to login.")]
        [SerializeField]
        private TMP_InputField _usernameText;
        /// <summary>
        /// SignIn button the user interacts with.
        /// </summary>
        [Tooltip("SignIn button the user interacts with.")]
        [SerializeField]
        private Button _signInButton;

        /// <summary>
        /// CanvasGroup on this object.
        /// </summary>
        private CanvasGroup _canvasGroup;
        /// <summary>
        /// SignInCanvas reference.
        /// </summary>
        private SignInCanvas _signInCanvas;
        private int old = 0;

        public void FirstInitialize(SignInCanvas signInCanvas)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _signInCanvas = signInCanvas;
            Reset();
            StartCoroutine(WaitForConnection());
        }

        /// <summary>
        /// Resets canvases as though first login.
        /// </summary>
        public void Reset()
        {
            SignInFailed(string.Empty);
        }

        /// <summary>
        /// Called when SignIn button is pressed.
        /// </summary>
        public void OnClick_SignIn()
        {
            string username = _usernameText.text.Trim();
            string failedReason = string.Empty;            
            //Local sanitization failed.
            if (!LobbyNetwork.SanitizeUsername(ref username, ref failedReason))
            {
                GlobalManager.CanvasesManager.MessagesCanvas.InfoMessages.ShowTimedMessage(failedReason, MessagesCanvas.BRIGHT_ORANGE);
            }
            //Try to login through server.
            else
            {
                SetSignInLocked(true);
                LobbyNetwork.SignIn(username);
            }
        }

        /// <summary>
        /// Sets locked state for signing in.
        /// </summary>
        /// <param name="locked"></param>
        private void SetSignInLocked(bool locked)
        {
            _signInButton.interactable = !locked;
            _usernameText.enabled = !locked;
        }

        /// <summary>
        /// Called after successfully signing in.
        /// </summary>
        public void SignInSuccess()
        {
            _canvasGroup.SetActive(false, true);
            SetSignInLocked(false);
        }

        /// <summary>
        /// Called after failing to sign in.
        /// </summary>
        public void SignInFailed(string failedReason)
        {
            _canvasGroup.SetActive(true, true);
            SetSignInLocked(false);

            if (failedReason != string.Empty)
                GlobalManager.CanvasesManager.MessagesCanvas.InfoMessages.ShowTimedMessage(failedReason, Color.red);
        }

        IEnumerator WaitForConnection(){
            yield return new WaitUntil(()=>InstanceFinder.ClientManager.Clients.Count > old);
            old = InstanceFinder.ClientManager.Clients.Count;
            PlayerData data = SaveSystem.LoadFromJson<PlayerData>();
            print("SignInMenu.cs");
            if(data != null && !string.IsNullOrWhiteSpace(data.username)){
                // wait until gamesettings has been confirmed
                LobbyNetwork.SignIn(data.username);

                print("success (PlayerData)");
            }
            else{
                print("No data found"); 
                yield return new WaitUntil(()=> ViewManager.Instance != null);
                yield return new WaitUntil(()=> ViewManager.Instance.currentView != null);
                yield return new WaitUntil(() => ViewManager.Instance.currentView.name != "SignInView");
                yield return new WaitForSeconds(1f);
                data = SaveSystem.LoadFromJson<PlayerData>();
                print("success (PlayerSettings)");
                
                LobbyNetwork.SignIn(data.username);
            }
        }
    }
}