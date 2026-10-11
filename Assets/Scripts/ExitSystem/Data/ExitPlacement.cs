using System;
using Game.BlockSystem;
using UnityEngine;

namespace Game.ExitSystem
{
    [Serializable]
    public struct ExitPlacement
    {
        [SerializeField] private Vector2Int _cell;
        [SerializeField] private ExitSide _side;
        [SerializeField, Min(1)] private int _length;
        [SerializeField] private BlockColorData _color;

        public Vector2Int Cell => _cell;
        public ExitSide Side => _side;
        public int Length => _length;
        public BlockColorData Color => _color;
    }
}
