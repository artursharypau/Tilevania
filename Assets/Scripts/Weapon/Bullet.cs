using UnityEngine;

namespace Weapon
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 15f;
        [SerializeField] private float _rotationSpeed = 300f;

        private Vector2 _direction;
        private LayerMask _targetLayerMask;
        private Rigidbody2D _rb;

        public bool IsFlying { get; private set; }

        private void Awake()
        {
            _direction = Vector2.zero;
            _targetLayerMask = LayerMask.GetMask("Enemy");
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            IsFlying = true;
            _rb.linearVelocity = _direction * _speed;
        }

        private void OnDisable()
        {
            Stop();
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
            Stop();

            if ((_targetLayerMask & (1 << other.gameObject.layer)) != 0)
            {
                gameObject.SetActive(false);
            }
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        private void Stop()
        {
            IsFlying = false;
            _rb.linearVelocity = Vector2.zero;
        }
    }
}
