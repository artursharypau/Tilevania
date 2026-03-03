using Common;
using Player;
using UnityEngine;

namespace Hazards
{
    public class HazardsController : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Player))
            {
                PlayerHealthController target = other.GetComponent<PlayerHealthController>();
                target.TakeDamage(target.Current, gameObject);
            }
        }
    }
}
