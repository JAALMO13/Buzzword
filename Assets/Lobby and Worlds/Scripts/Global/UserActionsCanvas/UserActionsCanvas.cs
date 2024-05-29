using FirstGearGames.LobbyAndWorld.Extensions;
using TMPro;
using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Global.Canvases
{

    public class UserActionsCanvas : MonoBehaviour
    {
        #region Serialized.
        /// <summary>
        /// Text to show signed in.
        /// </summary>
        [Tooltip("Text to show signed in.")]
        [SerializeField]
        private TextMeshProUGUI _signedInText;
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
        private const string SIGNED_IN_PREFIX = "Signed in as ";
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
            SignInFailed();
        }     

        /// <summary>
        /// Called when sign in passes.
        /// </summary>
        /// <param name="signedIn"></param>
        public void SignInSuccess(string username)
        {
            _canvasGroup.SetActive(true, true);
            _signedInText.text = SIGNED_IN_PREFIX + username;
        }

        /// <summary>
        /// Called when sign in fails.
        /// </summary>
        public void SignInFailed()
        {
            _canvasGroup.SetActive(false, true);
            _signedInText.text = string.Empty;
        }

    }

}