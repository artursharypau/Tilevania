using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class PlayerHealthController : MonoBehaviour
    {
        [SerializeField] private HealthUI _ui;

        private const int Max = 6;

        public int Current { get; private set; }
        public UnityEvent<GameObject> OnDeath { get; } = new();
        public UnityEvent<GameObject> OnDamageTook { get; } = new();

        public void Start()
        {
            Current = Max;

            _ui.UpdateHearts(Current);
        }

        public void TakeDamage(int amount, GameObject other)
        {
            Current -= amount;
            Current = Mathf.Clamp(Current, 0, Max);

            _ui.UpdateHearts(Current);
            OnDamageTook.Invoke(other);

            if (Current == 0)
            {
                OnDeath.Invoke(other);
            }
        }
    }
}
