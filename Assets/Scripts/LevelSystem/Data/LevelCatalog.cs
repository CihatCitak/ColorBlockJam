using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.LevelSystem
{
    [CreateAssetMenu(menuName = "Game/Level Catalog", fileName = "LevelCatalog")]
    internal class LevelCatalog : ScriptableObject
    {
        [SerializeField] private List<AssetReferenceT<LevelData>> _levels = new();

        public IReadOnlyList<AssetReferenceT<LevelData>> Levels => _levels;
    }
}
