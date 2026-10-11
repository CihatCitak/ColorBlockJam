using System;
using UnityEngine;

namespace Game.GameplaySystem
{
    [Serializable]
    internal struct PlayOnTier
    {
        [SerializeField, Min(0)] private int _cost;
        [SerializeField, Min(1)] private int _seconds;

        public PlayOnTier(int cost, int seconds)
        {
            _cost = cost;
            _seconds = seconds;
        }

        public int Cost => _cost;
        public int Seconds => _seconds;
    }
}
