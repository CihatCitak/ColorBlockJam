using System.Collections.Generic;
using UnityEngine;

namespace Game.GameplaySystem
{
    [CreateAssetMenu(menuName = "Game/Play On Config", fileName = "PlayOnConfig")]
    internal class PlayOnConfig : ScriptableObject
    {
        [SerializeField] private PlayOnTier[] _tiers =
        {
            new(900, 20),
            new(1900, 20),
            new(2900, 20)
        };

        public IReadOnlyList<PlayOnTier> Tiers => _tiers;
    }
}
