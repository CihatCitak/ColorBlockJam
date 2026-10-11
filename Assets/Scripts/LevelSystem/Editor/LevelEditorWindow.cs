using System;
using System.Collections.Generic;
using Game.BlockSystem;
using Game.ExitSystem;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Game.LevelSystem
{
    internal class LevelEditorWindow : EditorWindow
    {
        private const string Title = "Level Editor";
        private const string BoardHint = "Left click: use selected tool    Right click: erase";
        private const float ButtonWidth = 80f;
        private const float SectionSpacing = 16f;
        private const float BoardPadding = 20f;
        private const int MaxGridSize = 12;
        private const float GhostAlpha = 0.6f;

        private static readonly Color InvalidColor = new(1f, 0.2f, 0.2f, 0.5f);

        [SerializeField] private int _loadNumber = 1;
        [SerializeField] private int _saveNumber = 1;
        [SerializeField] private int _loadedNumber;
        [SerializeField] private bool _isDirty;
        [SerializeField] private LevelDraft _draft = new();
        [SerializeField] private LevelEditorTool _tool;
        [SerializeField] private int _shapeIndex;
        [SerializeField] private int _colorIndex;
        [SerializeField] private int _quarterTurns;
        [SerializeField] private Vector2 _scroll;

        private readonly List<DraftDoor> _doors = new();
        private readonly List<string> _warnings = new();
        private LevelPalette _palette;
        private LevelBoardDrawer _board;
        private LevelPaletteDrawer _paletteDrawer;

        private BlockColorData CurrentColor => _palette.Colors[_colorIndex];

        [MenuItem("ColorBlock/Level Editor")]
        private static void Open()
        {
            GetWindow<LevelEditorWindow>(Title);
        }

        private void OnEnable()
        {
            wantsMouseMove = true;
            _palette = BlockShapeFactory.LoadSingle<LevelPalette>();
            _board = new LevelBoardDrawer();
            _paletteDrawer = new LevelPaletteDrawer();
        }

        private void OnDisable()
        {
            _paletteDrawer.Dispose();
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.MouseDown)
                GUI.FocusControl(null);

            _shapeIndex = Mathf.Clamp(_shapeIndex, 0, _palette.Shapes.Count - 1);
            _colorIndex = Mathf.Clamp(_colorIndex, 0, _palette.Colors.Count - 1);
            _draft.CollectDoors(_doors);

            using var scroll = new EditorGUILayout.ScrollViewScope(_scroll);
            _scroll = scroll.scrollPosition;

            DrawLoad();
            GUILayout.Space(SectionSpacing);
            DrawSettings();
            EditorGUILayout.Space();
            DrawBoard();
            EditorGUILayout.HelpBox(BoardHint, MessageType.Info);
            EditorGUILayout.Space();
            DrawPalettes();
            EditorGUILayout.Space();
            DrawWarnings();
            GUILayout.Space(SectionSpacing);
            DrawSave();
        }

        [Shortcut("Level Editor/Rotate Shape", typeof(LevelEditorWindow), KeyCode.R)]
        private static void RotateShape(ShortcutArguments arguments)
        {
            var window = (LevelEditorWindow)arguments.context;
            window._quarterTurns = (window._quarterTurns + 1) % 4;
            window.Repaint();
        }

        private void DrawLoad()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _loadNumber = EditorGUILayout.IntField("Level Number", _loadNumber);
                using (new EditorGUI.DisabledScope(!IsExistingLevel(_loadNumber)))
                {
                    if (GUILayout.Button("Load", GUILayout.Width(ButtonWidth)))
                        Load();
                }
            }

            if (!IsExistingLevel(_loadNumber))
                EditorGUILayout.HelpBox($"Levels 1-{LevelAssetStore.Count} exist.", MessageType.Info);
        }

        private void DrawSettings()
        {
            EditorGUI.BeginChangeCheck();
            var size = EditorGUILayout.Vector2IntField("Grid Size", _draft.Size);
            var timeLimit = EditorGUILayout.IntField("Time (sec)", _draft.TimeLimit);
            if (!EditorGUI.EndChangeCheck())
                return;

            Record("Edit Level Settings");
            _draft.Resize(Vector2Int.Max(Vector2Int.one, Vector2Int.Min(size, Vector2Int.one * MaxGridSize)));
            _draft.TimeLimit = timeLimit;
        }

        private void DrawBoard()
        {
            _board.Layout(_draft.Size, position.width - BoardPadding);

            var current = Event.current;
            var hasCell = _board.TryGetCell(current.mousePosition, out var cell);
            var hasEdge = _board.TryGetEdge(current.mousePosition, _draft.IsDoorEdge, out var edgeCell, out var side);
            var targetsEdge = hasEdge && (!hasCell || TargetsEdge(current.button, edgeCell, side));

            switch (current.type)
            {
                case EventType.Repaint:
                    _board.Draw(_draft, _doors);
                    if (targetsEdge && _tool == LevelEditorTool.Door)
                        _board.DrawDoor(new DraftDoor(edgeCell, side, 1, CurrentColor), Ghost(CurrentColor.Color));
                    else if (hasCell && !targetsEdge)
                        DrawCellHover(cell);
                    break;
                case EventType.MouseMove:
                    Repaint();
                    break;
                case EventType.MouseDown when targetsEdge:
                    Record("Edit Level");
                    EditEdge(current.button, edgeCell, side);
                    current.Use();
                    break;
                case EventType.MouseDown when hasCell:
                    Record("Edit Level");
                    EditCell(current.button, cell);
                    current.Use();
                    break;
            }
        }

        private bool TargetsEdge(int button, Vector2Int edgeCell, ExitSide side)
        {
            if (button == 1)
                return _draft.HasDoor(edgeCell, side);

            return _tool is LevelEditorTool.Door or LevelEditorTool.Color;
        }

        private void DrawCellHover(Vector2Int cell)
        {
            if (_tool != LevelEditorTool.Shape)
            {
                _board.DrawHover(cell);
                return;
            }

            var block = NewBlock(cell);
            _board.DrawBlock(block, _draft.CanPlace(block) ? Ghost(CurrentColor.Color) : InvalidColor);
        }

        private void EditCell(int button, Vector2Int cell)
        {
            if (button == 1)
            {
                _draft.Erase(cell);
                return;
            }

            switch (_tool)
            {
                case LevelEditorTool.Shape:
                    _draft.TryAddBlock(NewBlock(cell));
                    break;
                case LevelEditorTool.Color:
                    var block = _draft.BlockAt(cell);
                    if (block >= 0)
                        _draft.SetBlockColor(block, CurrentColor);
                    break;
                case LevelEditorTool.Wall:
                    _draft.ToggleWall(cell);
                    break;
                case LevelEditorTool.Empty:
                    _draft.AddHole(cell);
                    break;
                case LevelEditorTool.Cell:
                    _draft.RemoveHole(cell);
                    break;
            }
        }

        private void EditEdge(int button, Vector2Int cell, ExitSide side)
        {
            if (button == 1)
                _draft.RemoveDoor(cell, side);
            else if (_tool == LevelEditorTool.Door)
                _draft.SetDoor(cell, side, CurrentColor);
            else if (_tool == LevelEditorTool.Color)
                _draft.SetDoorColor(cell, side, CurrentColor);
        }

        private void DrawPalettes()
        {
            var shapesLabel = $"Shapes    Rotation: {_quarterTurns * 90}° (press R)";
            var shape = _paletteDrawer.DrawShapes(shapesLabel, _palette.Shapes, _tool == LevelEditorTool.Shape ? _shapeIndex : -1);
            if (shape >= 0)
            {
                _tool = LevelEditorTool.Shape;
                _shapeIndex = shape;
            }

            var color = _paletteDrawer.DrawColors(_palette.Colors, _tool == LevelEditorTool.Color ? _colorIndex : -1);
            if (color >= 0)
            {
                _tool = LevelEditorTool.Color;
                _colorIndex = color;
            }

            var tile = _paletteDrawer.DrawTiles(Array.IndexOf(LevelPaletteDrawer.Tiles, _tool), CurrentColor.Color);
            if (tile >= 0)
                _tool = LevelPaletteDrawer.Tiles[tile];
        }

        private void DrawWarnings()
        {
            LevelValidator.Collect(_draft, _doors, _warnings);
            foreach (var warning in _warnings)
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
        }

        private void DrawSave()
        {
            var isValid = _saveNumber >= 1 && _saveNumber <= LevelAssetStore.Count + 1;
            using (new EditorGUILayout.HorizontalScope())
            {
                _saveNumber = EditorGUILayout.IntField("Save Level Number", _saveNumber);
                using (new EditorGUI.DisabledScope(!isValid))
                {
                    if (GUILayout.Button("Save", GUILayout.Width(ButtonWidth)))
                        Save();
                }
            }

            if (!isValid)
                EditorGUILayout.HelpBox($"Save Level Number must be between 1 and {LevelAssetStore.Count + 1}.", MessageType.Warning);
        }

        private void Load()
        {
            if (_isDirty && !EditorUtility.DisplayDialog(Title, "Discard unsaved changes?", "Discard", "Cancel"))
                return;

            Undo.RecordObject(this, "Load Level");
            _draft.Load(LevelAssetStore.Find(_loadNumber));
            _loadedNumber = _loadNumber;
            _saveNumber = _loadNumber;
            _isDirty = false;
            GUI.FocusControl(null);
        }

        private void Save()
        {
            if (_saveNumber != _loadedNumber && IsExistingLevel(_saveNumber) &&
                !EditorUtility.DisplayDialog(Title, $"Overwrite level {_saveNumber}?", "Overwrite", "Cancel"))
                return;

            LevelAssetStore.Save(_saveNumber, _draft);
            _loadedNumber = _saveNumber;
            _isDirty = false;
        }

        private void Record(string action)
        {
            Undo.RecordObject(this, action);
            _isDirty = true;
        }

        private DraftBlock NewBlock(Vector2Int cell)
        {
            return new DraftBlock(_palette.Shapes[_shapeIndex], CurrentColor, cell, _quarterTurns);
        }

        private static bool IsExistingLevel(int number)
        {
            return number >= 1 && number <= LevelAssetStore.Count;
        }

        private static Color Ghost(Color color)
        {
            return new Color(color.r, color.g, color.b, GhostAlpha);
        }
    }
}
