using UnityEngine;

namespace Game.GridSystem
{
    public readonly struct GridEdge
    {
        public readonly Vector2Int Cell;
        public readonly Vector2Int Direction;

        public GridEdge(Vector2Int cell, Vector2Int direction)
        {
            Cell = cell;
            Direction = direction;
        }
    }
}
