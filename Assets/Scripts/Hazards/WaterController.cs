using Common;
using Player;
using UnityEngine;

namespace Hazards
{
    public class WaterController : MonoBehaviour
    {
        [SerializeField] private PlayerLegs _playerLegs;

        private float _waterSurfaceTopPoint;
        private Collider2D _collider;

        private void Awake()
        {
            _waterSurfaceTopPoint = 0f;
            _collider = GetComponent<CompositeCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Player))
            {
                _waterSurfaceTopPoint = _collider.ClosestPoint(_playerLegs.transform.position).y;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Player))
            {
                float immersionDepth = _waterSurfaceTopPoint - _playerLegs.transform.position.y;
                if (immersionDepth >= 0.5f)
                {
                    PlayerHealthController target = other.GetComponent<PlayerHealthController>();
                    target.TakeDamage(target.Current, gameObject);
                }
            }
        }
    }
}
