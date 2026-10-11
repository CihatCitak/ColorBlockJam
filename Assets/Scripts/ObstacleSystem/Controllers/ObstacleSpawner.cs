using Game.Core.Services;
using Game.GridSystem;
using UnityEngine;

namespace Game.ObstacleSystem
{
    internal class ObstacleSpawner : BaseService, IObstacleSpawner
    {
        [SerializeField] private ObstacleConfig _config;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IObstacleSpawner>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IObstacleSpawner>(this);
        }

        public void Spawn(Vector2Int cell)
        {
            var position = ServiceLocator.Get<IGrid>().CellToWorld(cell);
            Instantiate(_config.ObstaclePrefab, position, Quaternion.identity, transform);
        }
    }
}
