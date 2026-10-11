using System;
using System.Collections.Generic;
using Game.BlockSystem;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.LevelSystem
{
    internal class LevelPaletteDrawer
    {
        private const float ItemSize = 56f;
        private const float ItemPadding = 6f;
        private const float FrameWidth = 3f;
        private const float DoorIconHeight = 0.25f;

        public static readonly LevelEditorTool[] Tiles = { LevelEditorTool.Door, LevelEditorTool.Wall, LevelEditorTool.Empty, LevelEditorTool.Cell };

        private static readonly Color FrameColor = Color.white;

        private readonly BlockShapePreview _preview = new();
        private readonly Dictionary<BlockShapeData, Texture> _thumbnails = new();

        public int DrawShapes(string label, IReadOnlyList<BlockShapeData> shapes, int selected)
        {
            return DrawRow(label, shapes.Count, selected, (rect, i) => GUI.DrawTexture(rect, Thumbnail(shapes[i]), ScaleMode.ScaleToFit));
        }

        public int DrawColors(IReadOnlyList<BlockColorData> colors, int selected)
        {
            return DrawRow("Colors", colors.Count, selected, (rect, i) => EditorGUI.DrawRect(rect, colors[i].Color));
        }

        public int DrawTiles(int selected, Color doorColor)
        {
            return DrawRow("Tiles", Tiles.Length, selected, (rect, i) => DrawTile(rect, Tiles[i], doorColor));
        }

        public void Dispose()
        {
            foreach (var thumbnail in _thumbnails.Values)
                Object.DestroyImmediate(thumbnail);

            _preview.Dispose();
        }

        private static int DrawRow(string label, int count, int selected, Action<Rect, int> drawItem)
        {
            var clicked = -1;
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                for (var i = 0; i < count; i++)
                {
                    var rect = GUILayoutUtility.GetRect(ItemSize, ItemSize, GUILayout.ExpandWidth(false));
                    if (GUI.Button(rect, GUIContent.none))
                        clicked = i;

                    drawItem(EditorRects.Inset(rect, ItemPadding), i);
                    if (i == selected)
                        EditorRects.Outline(rect, FrameColor, FrameWidth);
                }

                GUILayout.FlexibleSpace();
            }

            return clicked;
        }

        private static void DrawTile(Rect rect, LevelEditorTool tile, Color doorColor)
        {
            var label = new Rect(rect.x, rect.center.y, rect.width, rect.height * 0.5f);
            var icon = new Rect(rect.x, rect.y, rect.width, rect.height * 0.5f);
            GUI.Label(label, tile.ToString(), EditorStyles.centeredGreyMiniLabel);

            switch (tile)
            {
                case LevelEditorTool.Door:
                    EditorGUI.DrawRect(new Rect(icon.x, icon.center.y - icon.height * DoorIconHeight * 0.5f, icon.width, icon.height * DoorIconHeight), doorColor);
                    break;
                case LevelEditorTool.Wall:
                    EditorGUI.DrawRect(Square(icon), LevelBoardDrawer.WallColor);
                    break;
                case LevelEditorTool.Cell:
                    EditorGUI.DrawRect(Square(icon), LevelBoardDrawer.CellColor);
                    break;
                default:
                    EditorRects.Outline(Square(icon), FrameColor, 1f);
                    break;
            }
        }

        private Texture Thumbnail(BlockShapeData shape)
        {
            if (!_thumbnails.TryGetValue(shape, out var thumbnail))
            {
                var size = ItemSize * EditorGUIUtility.pixelsPerPoint;
                thumbnail = _preview.Capture(shape.Prefab.gameObject, new Rect(0f, 0f, size, size));
                thumbnail.hideFlags = HideFlags.HideAndDontSave;
                _thumbnails.Add(shape, thumbnail);
            }

            return thumbnail;
        }

        private static Rect Square(Rect rect)
        {
            var size = Mathf.Min(rect.width, rect.height);
            return new Rect(rect.center.x - size * 0.5f, rect.center.y - size * 0.5f, size, size);
        }
    }
}
