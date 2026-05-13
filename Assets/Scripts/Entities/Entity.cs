using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace GameJam
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Entity : SelectableEntity
    {
        /// <summary>
        /// When two entities collide, this event is triggered with the total number of entities involved in the collision (including the two that collided and any other entities that are currently colliding with them).
        /// </summary>
        public static event Action<int> OnEntitiesCollided = delegate { };
        public static event Action<Pedestrian> OnPedestrianCollided = delegate { };
        public event Action<Entity> OnEntityTimeStateChanged = delegate { };

        [Header("ENTITY SETTINGS")]
        [Header("Input")]
        [SerializeField] private InputActionReference increaseTimeAction;
        [SerializeField] private InputActionReference decreaseTimeAction;

        [Header("Stats")]
        [SerializeField] private float timeFactor;
        [Header("Lives")]
        [SerializeField] private int minLives = 1;
        [SerializeField] private int maxLives = 1;
        private int currentLives;
        [Space]
        [SerializeField] private float baseSpeed = 2f;

        [Header("Debug Info")]
        [SerializeField] private TimeState.TimeStateEnum currentTimeState = TimeState.TimeStateEnum.Normal;

        private Rigidbody _rb;

        public int Lives => currentLives;

        public TimeState.TimeStateEnum CurrentTimeState
        {
            get => currentTimeState;
            set
            {
                if(currentTimeState == value) return;
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

        private void Start()
        {
            currentLives = Random.Range(minLives, maxLives);
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
            if(collision.gameObject.TryGetComponent(out Entity otherEntity))
            {
                OnEntitiesCollided?.Invoke(currentLives);
                if(this is Pedestrian pedestrian)
                {
                    OnPedestrianCollided?.Invoke(pedestrian);
                }
                DestroyEntity();
            }
        }

        public void DestroyEntity()
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 2f);
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
    
