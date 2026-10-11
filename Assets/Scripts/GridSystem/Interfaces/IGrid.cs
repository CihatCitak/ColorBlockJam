using UnityEngine;

namespace Game.GridSystem
{
    public interface IGrid
    {
        Rect Area { get; }
        Vector2 CellToWorld(Vector2Int cell);
        Vector2Int WorldToCell(Vector2 world);
        Vector2 EdgeToWorld(GridEdge edge);
    }
}
