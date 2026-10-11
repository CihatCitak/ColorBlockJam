using Game.BlockSystem;
using UnityEngine;

namespace Game.ExitSystem
{
    internal static class ExitFit
    {
        public static bool Fits(BlockColorData blockColor, RectInt blockCells, BlockColorData exitColor, Vector2Int exitCell, int exitLength, Vector2Int direction)
        {
            if (blockColor != exitColor)
                return false;

            var exitStart = direction.x != 0 ? exitCell.y : exitCell.x;
            var blockStart = direction.x != 0 ? blockCells.yMin : blockCells.xMin;
            var blockEnd = direction.x != 0 ? blockCells.yMax : blockCells.xMax;
            return blockStart >= exitStart && blockEnd <= exitStart + exitLength;
        }
    }
}
