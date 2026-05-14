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
        [SerializeField] private int currentLives = 1;
        [SerializeField] private float baseSpeed = 2f;

        [Header("Force Settings")]
        [SerializeField] private float force = 15f;
        [SerializeField] private float upwardInfluence = 0.5f;
        [SerializeField] private ForceMode forceMode = ForceMode.Impulse;

        private Collider cachedCollider;

        [Header("Debug Info")]
        [SerializeField] private TimeState.TimeStateEnum currentTimeState = TimeState.TimeStateEnum.Normal;

        protected Rigidbody rb;

        public int Lives => currentLives;

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
            cachedCollider = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();

            rb.freezeRotation = true;

            increaseTimeAction.action.performed += OnIncreaseTimePerformed;
            decreaseTimeAction.action.performed += OnDecreaseTimePerformed;
        }

        private void FixedUpdate()
        {
            Move();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            increaseTimeAction.action.performed -= OnIncreaseTimePerformed;
            decreaseTimeAction.action.performed -= OnDecreaseTimePerformed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Entity otherEntity))
            {
                OnEntitiesCollided?.Invoke(currentLives);
            
                // Unica aggiunta: legge i layer per distinguere il tipo di scontro.
                // Il confronto instanceID su car→car evita il doppio fire
                // (OnCollisionEnter viene chiamato su entrambi i GameObject).
                int carLayer = LayerMask.NameToLayer("Car");
                int pedLayer = LayerMask.NameToLayer("Pedestrian");

                if (gameObject.layer == carLayer && collision.gameObject.layer == pedLayer)
                {
                    OnVehicleHitPedestrian?.Invoke(this, otherEntity);
                    Rigidbody victimRigidbody = collision.rigidbody;

                    if (victimRigidbody == null)
                        return;

                    Vector3 forwardDirection = transform.forward;
                    Vector3 upwardDirection = Vector3.up * upwardInfluence;

                    Vector3 finalDirection = (forwardDirection + upwardDirection).normalized;

                    victimRigidbody.AddForce(finalDirection * force, forceMode);

                    

                    Pedestrian ped = collision.gameObject.GetComponent<Pedestrian>();
                    ped.cachedCollider.enabled = false;
                    ped.PlayDeathAnimation();
                    collision.rigidbody.AddForce(collision.impulse);

                    ped.DestroyEntity();
                }
                else if (gameObject.layer == carLayer && collision.gameObject.layer == carLayer
                         && gameObject.GetInstanceID() < otherEntity.gameObject.GetInstanceID())
                {
                    OnVehicleHitVehicle?.Invoke(this, otherEntity);
                    
                }
                
                DestroyEntity();
            }
        }

        public virtual void DestroyEntity()
        {
            Debug.Log("Base destroy");
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
            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;
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