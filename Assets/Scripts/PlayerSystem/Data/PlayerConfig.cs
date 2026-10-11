using System;
using UnityEngine;

namespace Game.PlayerSystem
{
    [CreateAssetMenu(menuName = "Game/Player Config", fileName = "PlayerConfig")]
    internal class PlayerConfig : ScriptableObject
    {
        [SerializeField, Min(0)] private int _startCoins = 1000;
        [SerializeField, Min(1)] private int _maxLives = 5;
        [SerializeField, Min(1)] private int _lifeRefillMinutes = 30;

        public int StartCoins => _startCoins;
        public int MaxLives => _maxLives;
        public TimeSpan LifeRefillInterval => TimeSpan.FromMinutes(_lifeRefillMinutes);
    }
}
