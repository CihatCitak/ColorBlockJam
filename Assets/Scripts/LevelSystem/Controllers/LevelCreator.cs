using Cysharp.Threading.Tasks;
using Game.Core.Services;

namespace Game.LevelSystem
{
    internal class LevelCreator : BaseService, ILevelCreator
    {
        protected override void BindServices()
        {
            ServiceLocator.Bind<ILevelCreator>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<ILevelCreator>(this);
        }

        public UniTask CreateAsync(LevelData data)
        {
            return UniTask.CompletedTask;
        }
    }
}
