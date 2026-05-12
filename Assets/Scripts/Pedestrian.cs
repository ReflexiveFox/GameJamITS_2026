using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pedestrian : Entity
    {
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