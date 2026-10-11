using Game.Core.Services;
using Game.Core.Utilities;
using Game.GridSystem;
using UnityEngine;

namespace Game.ExitSystem
{
    internal class ExitSpawner : BaseService, IExitSpawner
    {
        private const float ArrowBaseAngle = -90f;

        [SerializeField] private ExitConfig _config;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IExitSpawner>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IExitSpawner>(this);
        }

        public void Spawn(ExitPlacement placement)
        {
            var grid = ServiceLocator.Get<IGrid>();
            var center = Vector2.zero;
            foreach (var edge in placement.Edges)
            {
                var position = grid.EdgeToWorld(edge);
                var rotation = Quaternion.Euler(0f, 0f, edge.Direction.x != 0 ? 90f : 0f);
                Instantiate(_config.DoorPrefab, position, rotation, transform).SetColor(placement.Color.Color);
                center += position;
            }

            var arrowRotation = Quaternion.Euler(0f, 0f, AngleUtility.ToDegrees(placement.Outward) - ArrowBaseAngle);
            Instantiate(_config.ArrowPrefab, center / placement.Length, arrowRotation, transform);
        }
    }
}
