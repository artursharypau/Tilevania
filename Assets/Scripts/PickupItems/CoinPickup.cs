using Common;
using Player;
using UI;
using UnityEngine;

namespace PickupItems
{
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private CoinPickupUI _ui;
        [SerializeField] private AudioClip _sfx;
        [SerializeField] private uint _coinValue = 100;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Player))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player)
                {
                    player.AddCoins(_coinValue);
                    _ui.UpdateUI(GameSession.Instance.CollectedCoins + player.Coins);
                }

                AudioSource.PlayClipAtPoint(_sfx, transform.position);

                Destroy(gameObject);
            }
        }
    }
}
