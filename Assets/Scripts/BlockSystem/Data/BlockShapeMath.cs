using System.Collections.Generic;
using UnityEngine;

namespace Game.BlockSystem
{
    internal static class BlockShapeMath
    {
        public static Vector2Int RotationOffset(IReadOnlyList<Vector2Int> cells, int quarterTurns)
        {
            var turns = ((quarterTurns % 4) + 4) % 4;
            var min = new Vector2Int(int.MaxValue, int.MaxValue);

            foreach (var cell in cells)
            {
                var rotated = cell;
                for (var i = 0; i < turns; i++)
                    rotated = new Vector2Int(rotated.y, -rotated.x);

                min = Vector2Int.Min(min, rotated);
            }

            return -min;
        }
    }
}
