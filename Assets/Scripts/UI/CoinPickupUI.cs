using TMPro;
using UnityEngine;

namespace UI
{
    public class CoinPickupUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        private uint _count;

        private void Start()
        {
            _scoreText.SetText(_count.ToString());
        }

        public void UpdateCount(uint count)
        {
            _count += count;
            _scoreText.SetText(_count.ToString());
        }
    }
}
