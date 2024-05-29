
using FishNet.Object;
using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Demos.KingOfTheHill
{

    public class ColorPlayer : NetworkBehaviour
    {
        /// <summary>
        /// Renderer for the player.
        /// </summary>
        [Tooltip("Renderer for the player.")]
        [SerializeField]
        private MeshRenderer _renderer;
        /// <summary>
        /// Material to use if owner.
        /// </summary>
        [Tooltip("Material to use if owner.")]
        [SerializeField]
        private Material _ownerMaterial;
        /// <summary>
        /// Material to use if not owner.
        /// </summary>
        [Tooltip("Material to use if not owner.")]
        [SerializeField]
        private Material _otherMaterial;



        public override void OnStartClient()
        {
            base.OnStartClient();

            Material[] mats = _renderer.sharedMaterials;
            Material material = (base.IsOwner) ? _ownerMaterial : _otherMaterial;
            for (int i = 0; i < mats.Length; i++)
                mats[i] = material;

            _renderer.sharedMaterials = mats;
        }
    }


}