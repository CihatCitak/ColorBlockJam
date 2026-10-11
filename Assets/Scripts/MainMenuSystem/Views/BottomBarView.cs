using System;
using DG.Tweening;
using UnityEngine;

namespace Game.MainMenuSystem
{
    internal class BottomBarView : MonoBehaviour
    {
        [SerializeField] private MainMenuConfig _config;
        [SerializeField] private TabView[] _tabs;
        [SerializeField] private RectTransform _highlight;

        public void Show(Action<int> onSelected)
        {
            for (var i = 0; i < _tabs.Length; i++)
            {
                var index = i;
                _tabs[i].Button.onClick.AddListener(() => onSelected(index));
            }
        }

        public void Select(int index)
        {
            for (var i = 0; i < _tabs.Length; i++)
                _tabs[i].SetSelected(i == index, _config);

            var target = ((RectTransform)_tabs[index].transform).anchoredPosition.x;
            _highlight.DOKill();
            DOTween.To(() => _highlight.anchoredPosition.x, MoveHighlight, target, _config.TabAnimationDuration)
                .SetEase(_config.TabAnimationEase)
                .SetTarget(_highlight)
                .SetLink(gameObject);
        }

        private void MoveHighlight(float x)
        {
            _highlight.anchoredPosition = new Vector2(x, _highlight.anchoredPosition.y);
        }
    }
}
