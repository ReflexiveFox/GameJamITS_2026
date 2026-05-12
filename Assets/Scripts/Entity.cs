using UnityEngine;

namespace GameJam
{
    public abstract class Entity : SelectableEntity
    {
        [SerializeField] private float timeFactor;
        [SerializeField] private int lives = 1;
        public int Lives => lives;

        private enum TimeState 
        {
             Slow = 0,
             Normal = 1,
             Fast = 2
        }

        public void ReduceTimeFactor()
        {

        }

        public void IncreaseTimeFactor()
        {

        }

        [SerializeField] private float baseSpeed = 2f;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            Vector3 velocity = transform.forward * baseSpeed;
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
        }
    }
}
    
