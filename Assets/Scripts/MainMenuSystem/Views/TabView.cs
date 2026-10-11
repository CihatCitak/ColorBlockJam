using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenuSystem
{
    internal class TabView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private RectTransform _icon;
        [SerializeField] private GameObject _label;

        private Vector2 _iconPosition;

        public Button Button => _button;

        private void Awake()
        {
            _iconPosition = _icon.anchoredPosition;
        }

        public void SetSelected(bool isSelected, MainMenuConfig config)
        {
            _label.SetActive(isSelected);
            _icon.DOKill();

            var scale = isSelected ? config.SelectedTabScale : 1f;
            var position = _iconPosition + (isSelected ? Vector2.up * config.SelectedTabRaise : Vector2.zero);
            _icon.DOScale(scale, config.TabAnimationDuration).SetEase(config.TabAnimationEase).SetLink(gameObject);
            DOTween.To(() => _icon.anchoredPosition, value => _icon.anchoredPosition = value, position, config.TabAnimationDuration)
                .SetEase(config.TabAnimationEase)
                .SetTarget(_icon)
                .SetLink(gameObject);
        }
    }
}
