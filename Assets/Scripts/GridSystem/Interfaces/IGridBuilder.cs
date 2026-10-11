using System.Collections.Generic;
using UnityEngine;

namespace Game.GridSystem
{
    public interface IGridBuilder
    {
        void Build(Vector2Int size, IReadOnlyList<Vector2Int> holes, IReadOnlyList<GridEdge> openings);
    }
}
