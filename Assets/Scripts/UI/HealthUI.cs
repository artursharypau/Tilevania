using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private List<Image> _heartImages;
        [SerializeField] private Sprite _emptyHeart;
        [SerializeField] private Sprite _halfHeart;
        [SerializeField] private Sprite _fullHeart;

        public void UpdateHearts(int current)
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
