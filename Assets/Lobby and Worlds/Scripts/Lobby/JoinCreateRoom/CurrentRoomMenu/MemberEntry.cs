using FirstGearGames.LobbyAndWorld.Clients;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases
{


    public class MemberEntry : MonoBehaviour
    {
        #region Public.
        /// <summary>
        /// Member this entry is for.
        /// </summary>
        public NetworkObject MemberId { get; private set; }
        /// <summary>
        /// PlayerSettings for the MemberId.
        /// </summary>
        public PlayerSettings PlayerSettings { get; private set; }
        #endregion

        #region Serialized.
        /// <summary>
        /// Icon to show if member is currently started.
        /// </summary>
        [Tooltip("Icon to show if member is currently started.")]
        [SerializeField]
        private Image _startedIcon;
        /// <summary>
        /// Name of member.
        /// </summary>
        [Tooltip("Name of member.")]
        [SerializeField]
        private TextMeshProUGUI _name;
        /// <summary>
        /// Kick button.
        /// </summary>
        [Tooltip("Kick button.")]
        [SerializeField]
        private GameObject _kickButton;
        #endregion

        #region Protected
        /// <summary>
        /// CurrentRoomMenu parenting this object.
        /// </summary>
        protected CurrentRoomMenu CurrentRoomMenu;
        #endregion

        /// <summary>
        /// Initializes this script for use. Should only be completed once.
        /// </summary>
        /// <param name="id"></param>
        public virtual void FirstInitialize(CurrentRoomMenu crm, NetworkObject id, bool started)
        {
            _startedIcon.gameObject.SetActive(started);
            CurrentRoomMenu = crm;
            MemberId = id;
            PlayerSettings = id.GetComponent<PlayerSettings>();

            _name.text = PlayerSettings.GetUsername();
        }

        /// <summary>
        /// Sets kick button active state.
        /// </summary>
        /// <param name="visible"></param>
        public void SetKickActive(bool active)
        {
            _kickButton.SetActive(active);
        }

        /// <summary>
        /// Called when Kick is pressed.
        /// </summary>
        public void OnClick_Kick()
        {
            CurrentRoomMenu.KickMember(this);
        }
    }


}