using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Global.Canvases
{


    public class CanvasesManager : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("MessagesCanvas reference.")]
        [SerializeField]
        private MessagesCanvas _messagesCanvas;
        /// <summary>
        /// MessagesCanvas reference.
        /// </summary>       
        public MessagesCanvas MessagesCanvas { get { return _messagesCanvas; } }
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("UserActionsCanvas reference.")]
        [SerializeField]
        private UserActionsCanvas _userActionsCanvas;
        /// <summary>
        /// UserActionsCanvas reference.
        /// </summary>       
        public UserActionsCanvas UserActionsCanvas { get { return _userActionsCanvas; } }
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("CurrentRoomCanvas reference.")]
        [SerializeField]
        private RoomActionsCanvas _roomActionCanvas;
        /// <summary>
        /// SignedInCanvas reference.
        /// </summary>       
        public RoomActionsCanvas RoomActionCanvas { get { return _roomActionCanvas; } }

    }


}