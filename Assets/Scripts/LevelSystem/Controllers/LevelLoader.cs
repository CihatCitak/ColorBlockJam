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

        public override void DeInitialize()
        {
            ReleaseLevel();
        }

        public async UniTask<LevelData> LoadAsync(int levelNumber)
        {
            if (_catalog.Count == 0)
                throw new InvalidOperationException("[LevelLoader] LevelCatalog is empty.");

            ReleaseLevel();
            _handle = Addressables.LoadAssetAsync<LevelData>(_catalog.GetByLevelNumber(levelNumber));
            return await _handle;
        }

        private void ReleaseLevel()
        {
            if (_handle.IsValid())
                Addressables.Release(_handle);
        }
    }
}
