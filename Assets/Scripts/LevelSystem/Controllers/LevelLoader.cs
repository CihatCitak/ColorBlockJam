using System;
using Cysharp.Threading.Tasks;
using Game.Core.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.LevelSystem
{
    internal class LevelLoader : BaseService, ILevelLoader
    {
        [SerializeField] private LevelCatalog _catalog;

        private AsyncOperationHandle<LevelData> _handle;

        protected override void BindServices()
        {
            ServiceLocator.Bind<ILevelLoader>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<ILevelLoader>(this);
        }

        protected override void DeInitialize()
        {
            ReleaseLevel();
        }

        public async UniTask<LevelData> LoadAsync(int levelNumber)
        {
            var levels = _catalog.Levels;
            if (levels.Count == 0)
                throw new InvalidOperationException("[LevelLoader] LevelCatalog is empty.");

            ReleaseLevel();
            _handle = Addressables.LoadAssetAsync<LevelData>(levels[CatalogIndex(levelNumber, levels.Count)]);
            return await _handle;
        }

        internal static int CatalogIndex(int levelNumber, int levelCount)
        {
            return (levelNumber - 1) % levelCount;
        }

        private void ReleaseLevel()
        {
            if (_handle.IsValid())
                Addressables.Release(_handle);
        }
    }
}
