using System;
using UnityEngine;

namespace Game.BlockSystem
{
    [Serializable]
    public struct BlockPlacement
    {
        [SerializeField] private BlockShapeData _shape;
        [SerializeField] private BlockColorData _color;
        [SerializeField] private Vector2Int _cell;
        [SerializeField, Range(0, 3)] private int _quarterTurns;

        public BlockShapeData Shape => _shape;
        public BlockColorData Color => _color;
        public Vector2Int Cell => _cell;
        public int QuarterTurns => _quarterTurns;
    }
}
