using Common;
using UI;
using UnityEngine;

namespace PickupItems
{
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private CoinPickupUI _ui;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Player))
            {
                _ui.UpdateCount(100);
                Destroy(gameObject);
            }
        }
    }
}
