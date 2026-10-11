using System;
using Game.BlockSystem;
using UnityEngine;

namespace Game.LevelSystem
{
    [Serializable]
    internal struct DraftBlock
    {
        [SerializeField] private BlockShapeData _shape;
        [SerializeField] private BlockColorData _color;
        [SerializeField] private Vector2Int _cell;
        [SerializeField] private int _quarterTurns;

        public DraftBlock(BlockShapeData shape, BlockColorData color, Vector2Int cell, int quarterTurns)
        {
            _shape = shape;
            _color = color;
            _cell = cell;
            _quarterTurns = quarterTurns;
        }

        public BlockShapeData Shape => _shape;
        public BlockColorData Color => _color;
        public Vector2Int Cell => _cell;
        public int QuarterTurns => _quarterTurns;
    }
}
