using FirstGearGames.LobbyAndWorld.Global.Canvases;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Global
{


    public class GlobalManager : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("GlobalCanvases reference.")]
        [SerializeField]
        private CanvasesManager _canvasesManager;
        /// <summary>
        /// GlobalCanvases reference.
        /// </summary>
        public static CanvasesManager CanvasesManager { get { return _instance._canvasesManager; } }

        /// <summary>
        /// Singleton reference to this script.
        /// </summary>
        private static GlobalManager _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }
    }


}