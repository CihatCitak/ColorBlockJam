using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.BlockSystem;
using Game.Core.Services;
using Game.ExitSystem;
using Game.GridSystem;
using Game.ObstacleSystem;

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
            var openings = new List<GridEdge>();
            foreach (var exit in data.Exits)
                openings.AddRange(exit.Edges);

            ServiceLocator.Get<IGridBuilder>().Build(data.GridSize, data.Holes, openings);

            var obstacles = ServiceLocator.Get<IObstacleSpawner>();
            foreach (var cell in data.Obstacles)
                obstacles.Spawn(cell);

            var exits = ServiceLocator.Get<IExitSpawner>();
            foreach (var exit in data.Exits)
                exits.Spawn(exit);

            var blocks = ServiceLocator.Get<IBlockSpawner>();
            foreach (var block in data.Blocks)
                blocks.Spawn(block);

            return UniTask.CompletedTask;
        }
    }
}
