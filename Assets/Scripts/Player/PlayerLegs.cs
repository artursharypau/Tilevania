using UnityEngine;

namespace Player
{
    public class PlayerLegs : MonoBehaviour
    {
        [SerializeField] private float _checkRadius = 0.1f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _checkRadius);
        }

        public Collider2D IsOnTheLayer(LayerMask mask)
        {
            return Physics2D.OverlapCircle(transform.position, _checkRadius, mask);
        }
    }
}
