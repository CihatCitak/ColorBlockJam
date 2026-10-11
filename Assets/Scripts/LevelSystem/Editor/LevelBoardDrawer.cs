using System;
using System.Collections.Generic;
using Game.ExitSystem;
using UnityEditor;
using UnityEngine;

namespace Game.LevelSystem
{
    internal class LevelBoardDrawer
    {
        public static readonly Color WallColor = new(0.25f, 0.25f, 0.28f);
        public static readonly Color CellColor = new(0.72f, 0.74f, 0.78f);

        private const int Margin = 1;
        private const float MinCellSize = 20f;
        private const float MaxCellSize = 48f;
        private const float CellGap = 1f;
        private const float WallInset = 0.12f;
        private const float DoorThickness = 0.3f;
        private const float LineWidth = 2f;

        private static readonly Color LineColor = new(0.1f, 0.1f, 0.12f);
        private static readonly Color HoverColor = Color.white;

        private static readonly Vector2Int[] Neighbours = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        private static readonly ExitSide[] SidesFacingBack = { ExitSide.Down, ExitSide.Up, ExitSide.Right, ExitSide.Left };

        private readonly List<Vector2Int> _footprint = new();
        private Rect _rect;
        private float _cellSize;
        private Vector2Int _size;

        public void Layout(Vector2Int size, float width)
        {
            _size = size;
            _cellSize = Mathf.Clamp(Mathf.Floor(width / (size.x + Margin * 2)), MinCellSize, MaxCellSize);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                _rect = GUILayoutUtility.GetRect((size.x + Margin * 2) * _cellSize, (size.y + Margin * 2) * _cellSize, GUILayout.ExpandWidth(false));
                GUILayout.FlexibleSpace();
            }
        }

        public bool TryGetCell(Vector2 point, out Vector2Int cell)
        {
            cell = GridPoint(point);
            return IsInside(cell);
        }

        public bool TryGetEdge(Vector2 point, Func<Vector2Int, ExitSide, bool> isDoorEdge, out Vector2Int cell, out ExitSide side)
        {
            var grid = GridPoint(point);
            var local = (point - _rect.position) / _cellSize;
            var offset = new Vector2(local.x - Mathf.Floor(local.x), local.y - Mathf.Floor(local.y));
            float[] distances = { offset.y, 1f - offset.y, offset.x, 1f - offset.x };

            cell = default;
            side = default;
            var nearest = float.MaxValue;
            for (var i = 0; i < Neighbours.Length; i++)
            {
                var neighbour = grid + Neighbours[i];
                if (distances[i] >= nearest || !isDoorEdge(neighbour, SidesFacingBack[i]))
                    continue;

                nearest = distances[i];
                cell = neighbour;
                side = SidesFacingBack[i];
            }

            return nearest < float.MaxValue;
        }

        public void Draw(LevelDraft draft, IReadOnlyList<DraftDoor> doors)
        {
            for (var y = 0; y < _size.y; y++)
            {
                for (var x = 0; x < _size.x; x++)
                {
                    var cell = new Vector2Int(x, y);
                    if (draft.IsCell(cell))
                        EditorGUI.DrawRect(EditorRects.Inset(CellRect(cell), CellGap), CellColor);
                }
            }

            foreach (var wall in draft.Walls)
                EditorGUI.DrawRect(EditorRects.Inset(CellRect(wall), _cellSize * WallInset), WallColor);

            foreach (var block in draft.Blocks)
                DrawBlock(block, block.Color.Color);

            foreach (var door in doors)
                DrawDoor(door, door.Color.Color);
        }

        public void DrawBlock(DraftBlock block, Color color)
        {
            LevelDraft.Footprint(block, _footprint);
            foreach (var cell in _footprint)
                EditorGUI.DrawRect(CellRect(cell), color);

            foreach (var cell in _footprint)
            {
                var rect = CellRect(cell);
                foreach (var neighbour in Neighbours)
                {
                    if (!_footprint.Contains(cell + neighbour))
                        EditorGUI.DrawRect(EdgeLine(rect, neighbour), LineColor);
                }
            }
        }

        public void DrawDoor(DraftDoor door, Color color)
        {
            var first = CellRect(door.Cell);
            var last = CellRect(door.Cell + LevelDraft.Along(door.Side) * (door.Length - 1));
            var span = Rect.MinMaxRect(Mathf.Min(first.xMin, last.xMin), Mathf.Min(first.yMin, last.yMin), Mathf.Max(first.xMax, last.xMax), Mathf.Max(first.yMax, last.yMax));
            var thickness = _cellSize * DoorThickness;

            var rect = door.Side switch
            {
                ExitSide.Up => new Rect(span.xMin + LineWidth, span.yMin - thickness - LineWidth, span.width - LineWidth * 2f, thickness),
                ExitSide.Down => new Rect(span.xMin + LineWidth, span.yMax + LineWidth, span.width - LineWidth * 2f, thickness),
                ExitSide.Left => new Rect(span.xMin - thickness - LineWidth, span.yMin + LineWidth, thickness, span.height - LineWidth * 2f),
                _ => new Rect(span.xMax + LineWidth, span.yMin + LineWidth, thickness, span.height - LineWidth * 2f)
            };

            EditorGUI.DrawRect(rect, color);
        }

        public void DrawHover(Vector2Int cell)
        {
            EditorRects.Outline(CellRect(cell), HoverColor, LineWidth);
        }

        private bool IsInside(Vector2Int cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < _size.x && cell.y < _size.y;
        }

        private Vector2Int GridPoint(Vector2 point)
        {
            var local = (point - _rect.position) / _cellSize;
            return new Vector2Int(Mathf.FloorToInt(local.x) - Margin, _size.y - 1 - (Mathf.FloorToInt(local.y) - Margin));
        }

        private Rect CellRect(Vector2Int cell)
        {
            return new Rect(_rect.x + (cell.x + Margin) * _cellSize, _rect.y + (_size.y - 1 - cell.y + Margin) * _cellSize, _cellSize, _cellSize);
        }

        private static Rect EdgeLine(Rect rect, Vector2Int direction)
        {
            if (direction == Vector2Int.up)
                return new Rect(rect.xMin, rect.yMin, rect.width, LineWidth);
            if (direction == Vector2Int.down)
                return new Rect(rect.xMin, rect.yMax - LineWidth, rect.width, LineWidth);
            if (direction == Vector2Int.left)
                return new Rect(rect.xMin, rect.yMin, LineWidth, rect.height);

            return new Rect(rect.xMax - LineWidth, rect.yMin, LineWidth, rect.height);
        }
    }
}
