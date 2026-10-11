using System;
using System.Collections.Generic;
using Game.BlockSystem;
using Game.ExitSystem;
using UnityEngine;

namespace Game.LevelSystem
{
    [Serializable]
    internal class LevelDraft
    {
        [SerializeField] private Vector2Int _size = new(5, 5);
        [SerializeField] private int _timeLimit = 60;
        [SerializeField] private List<Vector2Int> _holes = new();
        [SerializeField] private List<Vector2Int> _walls = new();
        [SerializeField] private List<DraftDoor> _doorEdges = new();
        [SerializeField] private List<DraftBlock> _blocks = new();

        private readonly List<Vector2Int> _footprint = new();
        private readonly List<Vector2Int> _placement = new();
        private readonly List<int> _run = new();
        private readonly HashSet<int> _merged = new();

        public Vector2Int Size => _size;
        public IReadOnlyList<Vector2Int> Holes => _holes;
        public IReadOnlyList<Vector2Int> Walls => _walls;
        public IReadOnlyList<DraftBlock> Blocks => _blocks;

        public int TimeLimit
        {
            get => _timeLimit;
            set => _timeLimit = Mathf.Max(1, value);
        }

        public static void Footprint(DraftBlock block, List<Vector2Int> result)
        {
            result.Clear();
            var origin = block.Cell + BlockShapeMath.RotationOffset(block.Shape.Cells, block.QuarterTurns);
            foreach (var cell in block.Shape.Cells)
            {
                var rotated = cell;
                for (var i = 0; i < block.QuarterTurns; i++)
                    rotated = new Vector2Int(rotated.y, -rotated.x);

                result.Add(origin + rotated);
            }
        }

        public static Vector2Int Along(ExitSide side)
        {
            var outward = ExitGeometry.Outward(side);
            return new Vector2Int(Mathf.Abs(outward.y), Mathf.Abs(outward.x));
        }

        public void Load(LevelData level)
        {
            _size = level.GridSize;
            _timeLimit = level.TimeLimit;
            _holes.Clear();
            _holes.AddRange(level.Holes);
            _walls.Clear();
            _walls.AddRange(level.Obstacles);

            _doorEdges.Clear();
            foreach (var exit in level.Exits)
            {
                foreach (var edge in ExitGeometry.Edges(exit))
                    _doorEdges.Add(new DraftDoor(edge.Cell, exit.Side, 1, exit.Color));
            }

            _blocks.Clear();
            foreach (var block in level.Blocks)
                _blocks.Add(new DraftBlock(block.Shape, block.Color, block.Cell, block.QuarterTurns));
        }

        public bool IsInside(Vector2Int cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < _size.x && cell.y < _size.y;
        }

        public bool IsCell(Vector2Int cell)
        {
            return IsInside(cell) && !_holes.Contains(cell);
        }

        public int BlockAt(Vector2Int cell)
        {
            for (var i = 0; i < _blocks.Count; i++)
            {
                Footprint(_blocks[i], _footprint);
                if (_footprint.Contains(cell))
                    return i;
            }

            return -1;
        }

        public bool CanPlace(DraftBlock block)
        {
            Footprint(block, _placement);
            foreach (var cell in _placement)
            {
                if (!IsCell(cell) || _walls.Contains(cell) || BlockAt(cell) >= 0)
                    return false;
            }

            return true;
        }

        public bool TryAddBlock(DraftBlock block)
        {
            if (!CanPlace(block))
                return false;

            _blocks.Add(block);
            return true;
        }

        public void SetBlockColor(int index, BlockColorData color)
        {
            var block = _blocks[index];
            _blocks[index] = new DraftBlock(block.Shape, color, block.Cell, block.QuarterTurns);
        }

        public void ToggleWall(Vector2Int cell)
        {
            if (_walls.Remove(cell) || !IsCell(cell) || BlockAt(cell) >= 0)
                return;

            _walls.Add(cell);
        }

        public void AddHole(Vector2Int cell)
        {
            if (!IsCell(cell) || BlockAt(cell) >= 0)
                return;

            _walls.Remove(cell);
            _holes.Add(cell);
            RemoveInvalidDoors();
        }

        public void RemoveHole(Vector2Int cell)
        {
            _holes.Remove(cell);
            RemoveInvalidDoors();
        }

        public void Erase(Vector2Int cell)
        {
            var block = BlockAt(cell);
            if (block >= 0)
                _blocks.RemoveAt(block);
            else if (!_walls.Remove(cell))
                _holes.Remove(cell);
        }

        public bool IsDoorEdge(Vector2Int cell, ExitSide side)
        {
            return IsCell(cell) && !IsCell(cell + ExitGeometry.Outward(side));
        }

        public bool HasDoor(Vector2Int cell, ExitSide side)
        {
            return DoorIndex(cell, side) >= 0;
        }

        public void SetDoor(Vector2Int cell, ExitSide side, BlockColorData color)
        {
            if (!IsDoorEdge(cell, side))
                return;

            var door = new DraftDoor(cell, side, 1, color);
            var index = DoorIndex(cell, side);
            if (index >= 0)
                _doorEdges[index] = door;
            else
                _doorEdges.Add(door);
        }

        public void SetDoorColor(Vector2Int cell, ExitSide side, BlockColorData color)
        {
            var index = DoorIndex(cell, side);
            if (index < 0)
                return;

            CollectRun(index);
            foreach (var i in _run)
                _doorEdges[i] = new DraftDoor(_doorEdges[i].Cell, side, 1, color);
        }

        public void RemoveDoor(Vector2Int cell, ExitSide side)
        {
            _doorEdges.RemoveAll(door => door.Cell == cell && door.Side == side);
        }

        public void CollectDoors(List<DraftDoor> result)
        {
            result.Clear();
            _merged.Clear();
            for (var i = 0; i < _doorEdges.Count; i++)
            {
                if (_merged.Contains(i))
                    continue;

                CollectRun(i);
                _merged.UnionWith(_run);
                var first = _doorEdges[_run[0]];
                result.Add(new DraftDoor(first.Cell, first.Side, _run.Count, first.Color));
            }
        }

        public void Resize(Vector2Int size)
        {
            _size = size;
            _holes.RemoveAll(cell => !IsInside(cell));
            _walls.RemoveAll(cell => !IsInside(cell));
            _blocks.RemoveAll(block => !IsInside(block));
            RemoveInvalidDoors();
        }

        private void RemoveInvalidDoors()
        {
            _doorEdges.RemoveAll(door => !IsDoorEdge(door.Cell, door.Side));
        }

        private bool IsInside(DraftBlock block)
        {
            Footprint(block, _footprint);
            return _footprint.TrueForAll(IsInside);
        }

        private void CollectRun(int index)
        {
            _run.Clear();
            var door = _doorEdges[index];
            var along = Along(door.Side);

            var start = door.Cell;
            while (RunIndex(start - along, door) >= 0)
                start -= along;

            int next;
            for (var cell = start; (next = RunIndex(cell, door)) >= 0; cell += along)
                _run.Add(next);
        }

        private int RunIndex(Vector2Int cell, DraftDoor door)
        {
            var index = DoorIndex(cell, door.Side);
            return index >= 0 && _doorEdges[index].Color == door.Color ? index : -1;
        }

        private int DoorIndex(Vector2Int cell, ExitSide side)
        {
            return _doorEdges.FindIndex(door => door.Cell == cell && door.Side == side);
        }
    }
}
