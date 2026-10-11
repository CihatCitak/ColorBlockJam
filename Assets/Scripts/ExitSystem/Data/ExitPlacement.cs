using System;
using System.Collections.Generic;
using Game.BlockSystem;
using Game.GridSystem;
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
        public int Length => Mathf.Max(1, _length);
        public BlockColorData Color => _color;

        public Vector2Int Outward => _side switch
        {
            ExitSide.Up => Vector2Int.up,
            ExitSide.Down => Vector2Int.down,
            ExitSide.Left => Vector2Int.left,
            _ => Vector2Int.right
        };

        public IEnumerable<GridEdge> Edges
        {
            get
            {
                var outward = Outward;
                var along = outward.x != 0 ? Vector2Int.up : Vector2Int.right;
                for (var i = 0; i < Length; i++)
                    yield return new GridEdge(_cell + along * i, outward);
            }
        }
    }
}
