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

        public static void ExitPath(IReadOnlyList<Vector2Int> cells, Vector2Int direction, List<Vector2Int> result)
        {
            result.Clear();
            var front = int.MinValue;
            foreach (var cell in cells)
                front = Mathf.Max(front, Dot(cell, direction));

            foreach (var cell in cells)
            {
                for (var step = cell + direction; Dot(step, direction) <= front; step += direction)
                {
                    if (!Contains(cells, step) && !result.Contains(step))
                        result.Add(step);
                }
            }
        }

        private static int Dot(Vector2Int a, Vector2Int b)
        {
            return a.x * b.x + a.y * b.y;
        }

        private static bool Contains(IReadOnlyList<Vector2Int> cells, Vector2Int cell)
        {
            foreach (var other in cells)
            {
                if (other == cell)
                    return true;
            }

            return false;
        }
    }
}
