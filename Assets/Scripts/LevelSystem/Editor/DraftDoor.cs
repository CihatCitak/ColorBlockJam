using System;
using Game.BlockSystem;
using Game.ExitSystem;
using UnityEngine;

namespace Game.LevelSystem
{
    [Serializable]
    internal struct DraftDoor
    {
        [SerializeField] private Vector2Int _cell;
        [SerializeField] private ExitSide _side;
        [SerializeField] private int _length;
        [SerializeField] private BlockColorData _color;

        public DraftDoor(Vector2Int cell, ExitSide side, int length, BlockColorData color)
        {
            _cell = cell;
            _side = side;
            _length = length;
            _color = color;
        }

        public Vector2Int Cell => _cell;
        public ExitSide Side => _side;
        public int Length => _length;
        public BlockColorData Color => _color;
    }
}
