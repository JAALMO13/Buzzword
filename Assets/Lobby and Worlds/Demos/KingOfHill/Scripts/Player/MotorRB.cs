
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;


namespace FirstGearGames.LobbyAndWorld.Demos.KingOfTheHill
{

    public class MotorRB : NetworkBehaviour
    {
        #region Types.
        public struct MoveData : IReplicateData
        {
            public float Horizontal;
            public float Vertical;
            public MoveData(float horizontal, float vertical)
            {
                Horizontal = horizontal;
                Vertical = vertical;
                _tick = 0;
            }

            private uint _tick;
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;

        }
        public struct ReconcileData : IReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;
            public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
                _tick = 0;
            }

            private uint _tick;
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }
        #endregion

        /// <summary>
        /// How quickly to accelerate.
        /// </summary>
        [Tooltip("How quickly to accelerate.")]
        [SerializeField]
        private float _acceleration = 10f;
        /// <summary>
        /// Rigidbody on this object.
        /// </summary>
        private Rigidbody _rigidbody;
        /// <summary>
        /// True if subscribed to events.
        /// </summary>
        private bool _subscribed = false;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            if (base.IsServer || base.Owner.IsLocalClient)
                SubscribeToEvents(true);
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool subscribe)
        {
            if (subscribe == _subscribed)
                return;
            if (base.TimeManager == null)
                return;

            _subscribed = subscribe;
            if (subscribe)
            {
                base.TimeManager.OnTick += TimeManager_OnTick;
                base.TimeManager.OnPostTick += TimeManager_OnPostTick;
            }
            else
            {
                base.TimeManager.OnTick -= TimeManager_OnTick;
                base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }


        private void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                Reconciliation(default, false);
                CheckInput(out MoveData md);
                Move(md, false);
            }
            if (base.IsServer)
            {
                Move(default, true);
            }
        }

        private void TimeManager_OnPostTick()
        {
            if (base.IsServer)
            {
                ReconcileData rd = new ReconcileData(transform.position, transform.rotation, _rigidbody.velocity, _rigidbody.angularVelocity);
                Reconciliation(rd, true);
            }
        }
        private void CheckInput(out MoveData md)
        {
            md = default;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal == 0f && vertical == 0f)
                return;

            md = new MoveData(horizontal, vertical);
        }

        [Replicate]
        private void Move(MoveData md, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
        {
            Vector3 forces = new Vector3(md.Horizontal, 0f, md.Vertical) * _acceleration;
            _rigidbody.AddForce(forces * (float)base.TimeManager.TickDelta);
        }

        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer, Channel channel = Channel.Unreliable)
        {
            transform.position = rd.Position;
            transform.rotation = rd.Rotation;
            _rigidbody.velocity = rd.Velocity;
            _rigidbody.angularVelocity = rd.AngularVelocity;
        }

    }


}