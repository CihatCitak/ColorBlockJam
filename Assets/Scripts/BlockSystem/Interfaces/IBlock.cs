using UnityEngine;

namespace Game.BlockSystem
{
    public interface IBlock
    {
        BlockColorData Color { get; }
        RectInt Cells { get; }
        void Exit(Vector2Int direction, Vector2 gate);
    }
}
