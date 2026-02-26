using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class HealthController : MonoBehaviour
    {
        private const int Max = 10;
        private int _current;

        public UnityEvent<int> OnHealthChanged { get; } = new();
        public UnityEvent<Vector2> OnDeath { get; } = new();

        public void Start()
        {
            _current = Max;

            OnHealthChanged.Invoke(_current);
        }

        public void TakeDamage(int amount, Vector2 position)
        {
            _current -= amount;
            _current = Mathf.Clamp(_current, 0, Max);

            OnHealthChanged.Invoke(_current);

            if (_current == 0)
            {
                OnDeath.Invoke(position);
            }
        }
    }
}
