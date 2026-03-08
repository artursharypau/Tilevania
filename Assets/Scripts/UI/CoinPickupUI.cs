using TMPro;
using UnityEngine;

namespace UI
{
    public class CoinPickupUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        private void Start()
        {
            GameSession session = FindFirstObjectByType<GameSession>();
            UpdateUI(session ? session.CollectedCoins : 0);
        }

        public void UpdateUI(uint count)
        {
            _scoreText.SetText(count.ToString());
        }
    }
}
