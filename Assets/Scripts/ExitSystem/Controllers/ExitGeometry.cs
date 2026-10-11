using System.Collections.Generic;
using Game.GridSystem;
using UnityEngine;

namespace Game.ExitSystem
{
    public static class ExitGeometry
    {
        public static Vector2Int Outward(ExitSide side)
        {
            return side switch
            {
                ExitSide.Up => Vector2Int.up,
                ExitSide.Down => Vector2Int.down,
                ExitSide.Left => Vector2Int.left,
                _ => Vector2Int.right
            };
        }

        public static IEnumerable<GridEdge> Edges(ExitPlacement placement)
        {
            var outward = Outward(placement.Side);
            var along = outward.x != 0 ? Vector2Int.up : Vector2Int.right;
            for (var i = 0; i < placement.Length; i++)
                yield return new GridEdge(placement.Cell + along * i, outward);
        }
    }
}
