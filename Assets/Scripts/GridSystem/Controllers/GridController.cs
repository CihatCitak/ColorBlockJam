using System.Collections.Generic;
using Game.Core.Services;
using UnityEngine;

namespace Game.GridSystem
{
    internal class GridController : BaseService, IGrid, IGridBuilder
    {
        [SerializeField] private GridConfig _config;

        private readonly HashSet<Vector2Int> _holes = new();
        private Vector2Int _size;

        private float CellSize => _config.CellSize;
        private float WallThickness => _config.WallThickness;

        public Rect Area
        {
            get
            {
                var size = (Vector2)_size * CellSize + Vector2.one * (WallThickness * 2f);
                return new Rect((Vector2)transform.position - size * 0.5f, size);
            }
        }

        protected override void BindServices()
        {
            ServiceLocator.Bind<IGrid>(this);
            ServiceLocator.Bind<IGridBuilder>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IGrid>(this);
            ServiceLocator.Unbind<IGridBuilder>(this);
        }

        public void Build(Vector2Int size, IReadOnlyList<Vector2Int> holes, IReadOnlyList<GridEdge> openings)
        {
            _size = size;
            _holes.Clear();
            _holes.UnionWith(holes);

            for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
            {
                var cell = new Vector2Int(x, y);
                if (!IsBoardCell(cell))
                    continue;

                Instantiate(_config.CellPrefab, CellToWorld(cell), Quaternion.identity, transform);
            }

            foreach (var piece in WallLayout.Build(size, IsBoardCell, openings, CellSize))
                Instantiate(_config.GetWallPrefab(piece.Type), (Vector2)transform.position + piece.Position, Quaternion.Euler(0f, 0f, piece.Rotation), transform);

            StaticBatchingUtility.Combine(gameObject);
        }

        public Vector2 CellToWorld(Vector2Int cell)
        {
            return (Vector2)transform.position + GridMath.CellToLocal(cell, _size, CellSize);
        }

        public Vector2Int WorldToCell(Vector2 world)
        {
            return GridMath.LocalToCell(world - (Vector2)transform.position, _size, CellSize);
        }

        public Vector2 EdgeToWorld(GridEdge edge)
        {
            return CellToWorld(edge.Cell) + (Vector2)edge.Direction * ((CellSize + WallThickness) * 0.5f);
        }

        private bool IsBoardCell(Vector2Int cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < _size.x && cell.y < _size.y && !_holes.Contains(cell);
        }
    }
}
