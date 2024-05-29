
using FishNet.Connection;
using FishNet.Object;
using System;


namespace FirstGearGames.LobbyAndWorld
{

    /// <summary>
    /// To be attached to localPlayer prefabs to know when they are instantiated.
    /// </summary>
    public class LocalPlayerAnnouncer : NetworkBehaviour
    {

        #region Public.
        /// <summary>
        /// Dispatched when the local player changes, providing the new localPlayer.
        /// </summary>
        public static event Action<NetworkObject> OnLocalPlayerUpdated;
        #endregion

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);
            if (base.IsOwner)
                OnLocalPlayerUpdated?.Invoke(base.NetworkObject);
        }

        private void OnEnable()
        {
            if (base.IsOwner)
                OnLocalPlayerUpdated?.Invoke(base.NetworkObject);
        }
        private void OnDisable()
        {
            if (base.IsOwner)
                OnLocalPlayerUpdated?.Invoke(null);
        }
    }


}