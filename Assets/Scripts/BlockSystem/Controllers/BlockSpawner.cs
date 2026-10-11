using System;
using Game.Core.Services;
using Game.GridSystem;
using UnityEngine;

namespace Game.BlockSystem
{
    internal class BlockSpawner : BaseService, IBlockSpawner
    {
        private const float QuarterTurnAngle = 90f;

        [SerializeField] private BlockConfig _config;
        [SerializeField] private ParticleSystem _crushEffect;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IBlockSpawner>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IBlockSpawner>(this);
        }

        public void Spawn(BlockPlacement placement)
        {
            var shape = placement.Shape;
            if (shape.Prefab == null)
                throw new InvalidOperationException($"[BlockSpawner] {shape.name} has no prefab. Create shapes with ColorBlock/Shape Creator.");

            var rotation = Quaternion.Euler(0f, 0f, -QuarterTurnAngle * placement.QuarterTurns);
            var block = Instantiate(shape.Prefab, transform.position, rotation, transform);
            var originCell = placement.Cell + BlockShapeMath.RotationOffset(shape.Cells, placement.QuarterTurns);
            block.Build(placement.Color, originCell, ServiceLocator.Get<IGrid>(), _config, _crushEffect);
        }
    }
}
