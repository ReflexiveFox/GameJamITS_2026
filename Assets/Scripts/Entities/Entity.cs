using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Entity : SelectableEntity
    {
        public static event Action<int> OnEntitiesCollided = delegate { };
        public event Action<Entity> OnEntityTimeStateChanged = delegate { };

        // Unica aggiunta: due eventi statici per distinguere il tipo di collisione.
        // Vengono fired dentro OnCollisionEnter che esiste già, usando i layer
        // "car" e "pedestrian" già configurati nel progetto.
        public static event Action<Entity, Entity> OnVehicleHitPedestrian = delegate { };
        public static event Action<Entity, Entity> OnVehicleHitVehicle = delegate { };

        [Header("ENTITY SETTINGS")]
        [Header("Input")]
        [SerializeField] private InputActionReference increaseTimeAction;
        [SerializeField] private InputActionReference decreaseTimeAction;

        [Header("Stats")]
        [SerializeField] private float timeFactor;
        [SerializeField] private int lives = 1;
        [SerializeField] private float baseSpeed = 2f;

        [Header("Debug Info")]
        [SerializeField] private TimeState.TimeStateEnum currentTimeState = TimeState.TimeStateEnum.Normal;

        private Rigidbody _rb;

        public int Lives => lives;

        public TimeState.TimeStateEnum CurrentTimeState
        {
            get => currentTimeState;
            set
            {
                if (currentTimeState == value) return;
                currentTimeState = value;
                OnEntityTimeStateChanged?.Invoke(this);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;

            increaseTimeAction.action.performed += OnIncreaseTimePerformed;
            decreaseTimeAction.action.performed += OnDecreaseTimePerformed;
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void OnDestroy()
        {
            increaseTimeAction.action.performed -= OnIncreaseTimePerformed;
            decreaseTimeAction.action.performed -= OnDecreaseTimePerformed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Entity otherEntity))
            {
                OnEntitiesCollided?.Invoke(lives); // invariato

                // Unica aggiunta: legge i layer per distinguere il tipo di scontro.
                // Il confronto instanceID su car→car evita il doppio fire
                // (OnCollisionEnter viene chiamato su entrambi i GameObject).
                int carLayer = LayerMask.NameToLayer("Car");
                int pedLayer = LayerMask.NameToLayer("Pedestrian");

                if (gameObject.layer == carLayer && collision.gameObject.layer == pedLayer)
                    OnVehicleHitPedestrian?.Invoke(this, otherEntity);
                else if (gameObject.layer == carLayer && collision.gameObject.layer == carLayer
                         && gameObject.GetInstanceID() < otherEntity.gameObject.GetInstanceID())
                    OnVehicleHitVehicle?.Invoke(this, otherEntity);

                gameObject.SetActive(false); // invariato
                Destroy(gameObject, 2f);     // invariato
            }
        }

        private void OnIncreaseTimePerformed(InputAction.CallbackContext context)
        {
            if (!IsSelected) return;
            IncreaseTimeFactor();
        }

        private void OnDecreaseTimePerformed(InputAction.CallbackContext context)
        {
            if (!IsSelected) return;
            ReduceTimeFactor();
        }

        private void Move()
        {
            Vector3 velocity = transform.forward * baseSpeed * TimeState.Instance.GetTimeFactor(CurrentTimeState);
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
        }

        public void ReduceTimeFactor()
        {
            CurrentTimeState = (TimeState.TimeStateEnum)Mathf.Clamp((int)CurrentTimeState - 1, (int)TimeState.TimeStateEnum.Slow, (int)TimeState.TimeStateEnum.Fast);
        }

        public void IncreaseTimeFactor()
        {
            CurrentTimeState = (TimeState.TimeStateEnum)Mathf.Clamp((int)CurrentTimeState + 1, (int)TimeState.TimeStateEnum.Slow, (int)TimeState.TimeStateEnum.Fast);
        }
    }
}