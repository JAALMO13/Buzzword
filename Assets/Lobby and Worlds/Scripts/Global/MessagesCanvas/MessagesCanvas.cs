using FirstGearGames.LobbyAndWorld.Clients;
using FishNet.Object;
using System.Collections;
using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Global.Canvases
{

    public class MessagesCanvas : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("MessagesMenu for info messages.")]
        [SerializeField]
        private MessageMenu _infoMessages;
        /// <summary>
        /// MessagesMenu for info messages.
        /// </summary>
        public MessageMenu InfoMessages { get { return _infoMessages; } }
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("MessagesMenu for full messages.")]
        [SerializeField]
        private MessageMenu _fullMessages;
        /// <summary>
        /// MessagesMenu for full messages.
        /// </summary>
        public MessageMenu FullMessages { get { return _fullMessages; } }


        /// <summary>
        /// Coroutine which hides the FullMessages canvases when ClientInstance becomes initialized.
        /// </summary>
        private Coroutine _hideOnInitialized = null;

        #region Colors.
        public static readonly Color LIGHT_BLUE = new Color(0.35f, 0.65f, 1f, 1f);
        public static readonly Color BRIGHT_ORANGE = new Color(1f, 0.6f, 0.3f, 1f);
        #endregion

        private void Start()
        {
            FirstInitialize();
        }

        /// <summary>
        /// Initializes this script for use. Should only be completed once.
        /// </summary>
        public void FirstInitialize()
        {
            LocalPlayerAnnouncer.OnLocalPlayerUpdated += LocalPlayerAnnouncer_OnLocalPlayerUpdated;
            Reset();
        }

        private void OnDestroy()
        {
            LocalPlayerAnnouncer.OnLocalPlayerUpdated -= LocalPlayerAnnouncer_OnLocalPlayerUpdated;
        }

        /// <summary>
        /// Resets canvases as though first login.
        /// </summary>
        public void Reset()
        {
            InfoMessages.EndPersistentText();
            FullMessages.StartPersistentText("Waiting for connection to server.", Color.black);
            StopHideOnInitialized();
        }

        /// <summary>
        /// Received once localPlayer is created.
        /// </summary>
        /// <param name="obj"></param>
        private void LocalPlayerAnnouncer_OnLocalPlayerUpdated(NetworkObject obj)
        {
            StopHideOnInitialized();
            if (obj == null)
            { 
                FullMessages.StartPersistentText("Waiting for connection to server.", Color.black);
            }
            else
            {
                _hideOnInitialized = StartCoroutine(__HideOnInitializedClientInstance(obj));
            }
        }

        /// <summary>
        /// Stops the HideOnInitialized coroutine.
        /// </summary>
        private void StopHideOnInitialized()
        {
            if (_hideOnInitialized != null)
            {
                StopCoroutine(_hideOnInitialized);
                _hideOnInitialized = null;
            }
        }

        /// <summary>
        /// Hides FullMessages menu after ClientInstance is initialized.
        /// </summary>
        /// <returns></returns>
        private IEnumerator __HideOnInitializedClientInstance(NetworkObject obj)
        {
            ClientInstance ci = obj.GetComponent<ClientInstance>();
            while (!ci.Initialized)
                yield return null;

            FullMessages.EndPersistentText();
            _hideOnInitialized = null;
        }
    }


}