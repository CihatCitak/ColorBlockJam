using System;
using System.Collections.Generic;
using Game.Core.Utilities;
using UnityEngine;

namespace Game.GridSystem
{
    internal enum WallPieceType
    {
        Wall,
        Corner,
        LCorner
    }

    internal readonly struct WallPiece
    {
        public readonly Vector2 Position;
        public readonly WallPieceType Type;
        public readonly float Rotation;

        public WallPiece(Vector2 position, WallPieceType type, float rotation)
        {
            Position = position;
            Type = type;
            Rotation = rotation;
        }
    }

    internal static class WallLayout
    {
        private const float CornerBaseAngle = 45f;
        private const float HorizontalWallAngle = 90f;

        private static readonly Vector2Int[] Diagonal = { new(1, 1), new(-1, 1), new(1, -1), new(-1, -1) };

        public static List<WallPiece> Build(Vector2Int size, Func<Vector2Int, bool> isBoardCell, IEnumerable<GridEdge> openings, float cellSize)
        {
            var pieces = new List<WallPiece>();
            var taken = new HashSet<Vector2Int>();

            foreach (var opening in openings)
            {
                var first = opening.Cell * 2 + new Vector2Int(opening.Direction.x > 0 ? 2 : opening.Direction.x, opening.Direction.y > 0 ? 2 : opening.Direction.y);
                taken.Add(first);
                taken.Add(first + new Vector2Int(Mathf.Abs(opening.Direction.y), Mathf.Abs(opening.Direction.x)));
            }

            for (var x = -1; x <= size.x * 2; x++)
            for (var y = -1; y <= size.y * 2; y++)
            {
                var quarter = new Vector2Int(x, y);
                if (taken.Contains(quarter) || IsBoardQuarter(quarter, isBoardCell) || !TryGetDiagonalBoard(quarter, isBoardCell, out var towardBoard))
                    continue;

                var position = QuarterToLocal(quarter, size, cellSize);
                var rotation = AngleUtility.ToDegrees(-towardBoard) - CornerBaseAngle;
                var armX = quarter + new Vector2Int(towardBoard.x, 0);
                var armY = quarter + new Vector2Int(0, towardBoard.y);

                taken.Add(quarter);
                if (taken.Contains(armX) || taken.Contains(armY))
                {
                    pieces.Add(new WallPiece(position, WallPieceType.Corner, rotation));
                    continue;
                }

                taken.Add(armX);
                taken.Add(armY);
                pieces.Add(new WallPiece(position, WallPieceType.LCorner, rotation));
            }

            for (var x = -1; x <= size.x * 2; x++)
            for (var y = -1; y <= size.y * 2; y++)
            {
                var quarter = new Vector2Int(x, y);
                if (taken.Contains(quarter) || IsBoardQuarter(quarter, isBoardCell))
                    continue;

                var towardBoard = new Vector2Int(BoardSide(quarter, Vector2Int.right, isBoardCell), BoardSide(quarter, Vector2Int.up, isBoardCell));
                if (towardBoard == Vector2Int.zero)
                    continue;

                var position = QuarterToLocal(quarter, size, cellSize);
                if (towardBoard.x != 0 && towardBoard.y != 0)
                    pieces.Add(new WallPiece(position, WallPieceType.Corner, AngleUtility.ToDegrees(towardBoard) - CornerBaseAngle));
                else
                    pieces.Add(new WallPiece(position, WallPieceType.Wall, towardBoard.y != 0 ? HorizontalWallAngle : 0f));
            }

            return pieces;
        }

        private static bool TryGetDiagonalBoard(Vector2Int quarter, Func<Vector2Int, bool> isBoardCell, out Vector2Int direction)
        {
            direction = default;
            if (BoardSide(quarter, Vector2Int.right, isBoardCell) != 0 || BoardSide(quarter, Vector2Int.up, isBoardCell) != 0)
                return false;

            foreach (var d in Diagonal)
            {
                if (!IsBoardQuarter(quarter + d, isBoardCell))
                    continue;

                direction = d;
                return true;
            }

            return false;
        }

        private static int BoardSide(Vector2Int quarter, Vector2Int axis, Func<Vector2Int, bool> isBoardCell)
        {
            if (IsBoardQuarter(quarter + axis, isBoardCell))
                return 1;

            return IsBoardQuarter(quarter - axis, isBoardCell) ? -1 : 0;
        }

        private static bool IsBoardQuarter(Vector2Int quarter, Func<Vector2Int, bool> isBoardCell)
        {
            return isBoardCell(new Vector2Int(Mathf.FloorToInt(quarter.x * 0.5f), Mathf.FloorToInt(quarter.y * 0.5f)));
        }

        private static Vector2 QuarterToLocal(Vector2Int quarter, Vector2Int size, float cellSize)
        {
            return new Vector2((quarter.x + 0.5f) * 0.5f - size.x * 0.5f, (quarter.y + 0.5f) * 0.5f - size.y * 0.5f) * cellSize;
        }
    }
}
