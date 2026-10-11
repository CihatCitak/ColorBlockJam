using DG.Tweening;
using UnityEngine;

namespace Game.MainMenuSystem
{
    [CreateAssetMenu(menuName = "Game/Main Menu Config", fileName = "MainMenuConfig")]
    internal class MainMenuConfig : ScriptableObject
    {
        [SerializeField, Min(1f)] private float _selectedTabScale = 1.6f;
        [SerializeField] private float _selectedTabRaise = 60f;
        [SerializeField, Min(0f)] private float _tabAnimationDuration = 0.2f;
        [SerializeField] private Ease _tabAnimationEase = Ease.OutBack;
        [SerializeField, Min(0)] private int _refillLivesCost = 900;
        [SerializeField, Min(1)] private int _adLives = 1;

        public float SelectedTabScale => _selectedTabScale;
        public float SelectedTabRaise => _selectedTabRaise;
        public float TabAnimationDuration => _tabAnimationDuration;
        public Ease TabAnimationEase => _tabAnimationEase;
        public int RefillLivesCost => _refillLivesCost;
        public int AdLives => _adLives;
    }
}
