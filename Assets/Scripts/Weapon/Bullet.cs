using UnityEngine;

namespace Weapon
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 15f;
        [SerializeField] private float _rotationSpeed = 500f;

        private Rigidbody2D _rb;
        private Vector2 _direction;

        public bool IsFlying { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            IsFlying = true;
            _rb.linearVelocity = _direction * _speed;
        }

        private void Update()
        {
            if (IsFlying)
            {
                transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            IsFlying = false;
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }
    }
}
