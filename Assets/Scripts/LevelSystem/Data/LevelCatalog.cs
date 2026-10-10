using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.LevelSystem
{
    [CreateAssetMenu(menuName = "Game/Level Catalog", fileName = "LevelCatalog")]
    internal class LevelCatalog : ScriptableObject
    {
        [SerializeField] private List<AssetReferenceT<LevelData>> _levels = new();

        public int Count => _levels.Count;

        public AssetReferenceT<LevelData> GetByLevelNumber(int levelNumber)
        {
            return _levels[(levelNumber - 1) % _levels.Count];
        }
    }
}
