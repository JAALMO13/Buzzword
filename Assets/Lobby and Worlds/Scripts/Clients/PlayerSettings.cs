

using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Clients
{

    public class PlayerSettings : NetworkBehaviour
    {

        // get avatar
        // colour chosen and first letter in name
        #region Private.
        /// <summary>
        /// Username for this client.
        /// </summary>
        [SyncVar]
        private string _username;

        [SyncVar]
        private Color _colour;
        #endregion

        /// <summary>
        /// Sets Username.
        /// </summary>
        /// <param name="value"></param>
        private void Start() {
            Debug.LogWarning("Colour set Randomly");
            _colour = Random.ColorHSV();
        }
        public void SetUsername(string value)
        {
            _username = value;
        }
        /// <summary>
        /// Returns Username.
        /// </summary>
        /// <returns></returns>
        public string GetUsername() 
        {
            return _username; 
        }
        /// <summary>
        /// Sets Colour.
        /// </summary>
        /// <param name="value"></param>
        public void SetColour(Color value)
        {
            _colour = value;
        }
        /// <summary>
        /// Returns Colour chosen.
        /// </summary>
        /// <returns></returns>
        public Color GetColour() 
        {
            return _colour; 
        }
    }

}
