using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private List<Image> _heartImages;
        [SerializeField] private Sprite _emptyHeart;
        [SerializeField] private Sprite _halfHeart;
        [SerializeField] private Sprite _fullHeart;
        [SerializeField] private HealthController _healthController;

        private void OnEnable()
        {
            _healthController.OnHealthChanged.AddListener(UpdateHearts);
        }

        private void OnDisable()
        {
            _healthController.OnHealthChanged.RemoveListener(UpdateHearts);
        }

        private void UpdateHearts(int current)
        {
            for (int i = 0; i < _heartImages.Count; i++)
            {
                int heartValue = i * 2;

                if (current >= heartValue + 2)
                {
                    _heartImages[i].sprite = _fullHeart;
                }
                else if (current == heartValue + 1)
                {
                    _heartImages[i].sprite = _halfHeart;
                }
                else
                {
                    _heartImages[i].sprite = _emptyHeart;
                }
            }
        }
    }
}
