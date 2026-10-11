using System.Collections.Generic;
using Game.Core.Utilities;
using UnityEngine;

namespace Game.BlockSystem
{
    internal enum BlockPartType
    {
        Center,
        Edge,
        OuterCorner,
        InnerCorner
    }

    internal readonly struct BlockPart
    {
        public readonly BlockPartType Type;
        public readonly Vector2 Position;
        public readonly float Rotation;

        public BlockPart(BlockPartType type, Vector2 position, float rotation)
        {
            Type = type;
            Position = position;
            Rotation = rotation;
        }
    }

    internal static class BlockPartLayout
    {
        private const float EdgeBaseAngle = 90f;
        private const float CornerBaseAngle = 45f;

        public static List<BlockPart> Build(IEnumerable<Vector2Int> cells, float cellSize)
        {
            var filled = new HashSet<Vector2Int>(cells);
            var vertices = new HashSet<Vector2Int>();
            foreach (var cell in filled)
            {
                vertices.Add(cell);
                vertices.Add(cell + Vector2Int.right);
                vertices.Add(cell + Vector2Int.up);
                vertices.Add(cell + Vector2Int.one);
            }

            var parts = new List<BlockPart>();
            foreach (var vertex in vertices)
                AddPartsAround(vertex, filled, cellSize, parts);

            return parts;
        }

        private static void AddPartsAround(Vector2Int vertex, HashSet<Vector2Int> filled, float cellSize, List<BlockPart> parts)
        {
            var vertexPosition = (vertex - new Vector2(0.5f, 0.5f)) * cellSize;
            var emptyDirection = Vector2Int.zero;
            var filledCount = 0;

            for (var dx = -1; dx <= 1; dx += 2)
            for (var dy = -1; dy <= 1; dy += 2)
            {
                if (filled.Contains(QuadrantCell(vertex, dx, dy)))
                    filledCount++;
                else
                    emptyDirection = new Vector2Int(dx, dy);
            }

            if (filledCount == 3)
            {
                parts.Add(new BlockPart(BlockPartType.InnerCorner, vertexPosition, AngleUtility.ToDegrees(emptyDirection) - CornerBaseAngle));
                return;
            }

            for (var dx = -1; dx <= 1; dx += 2)
            for (var dy = -1; dy <= 1; dy += 2)
            {
                if (!filled.Contains(QuadrantCell(vertex, dx, dy)))
                    continue;

                var horizontal = filled.Contains(QuadrantCell(vertex, -dx, dy));
                var vertical = filled.Contains(QuadrantCell(vertex, dx, -dy));
                var position = vertexPosition + new Vector2(dx, dy) * (cellSize * 0.25f);
                var outward = new Vector2Int(-dx, -dy);

                if (horizontal && vertical)
                    parts.Add(new BlockPart(BlockPartType.Center, position, 0f));
                else if (horizontal)
                    parts.Add(new BlockPart(BlockPartType.Edge, position, AngleUtility.ToDegrees(new Vector2Int(0, outward.y)) - EdgeBaseAngle));
                else if (vertical)
                    parts.Add(new BlockPart(BlockPartType.Edge, position, AngleUtility.ToDegrees(new Vector2Int(outward.x, 0)) - EdgeBaseAngle));
                else
                    parts.Add(new BlockPart(BlockPartType.OuterCorner, position, AngleUtility.ToDegrees(outward) - CornerBaseAngle));
            }
        }

        private static Vector2Int QuadrantCell(Vector2Int vertex, int dx, int dy)
        {
            return new Vector2Int(dx > 0 ? vertex.x : vertex.x - 1, dy > 0 ? vertex.y : vertex.y - 1);
        }
    }
}
