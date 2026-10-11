using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.BlockSystem
{
    internal class ShapeCreator : EditorWindow
    {
        private const int GridSize = BlockShapePreview.GridSize;
        private const float CellButtonSize = 32f;
        private const float PreviewSize = GridSize * CellButtonSize;
        private const float SectionSpacing = 20f;
        private const string GridHint = "Left click: add or remove a cell    Right click: remove a cell";

        private static readonly Color FilledColor = new(0.3f, 0.8f, 0.4f);
        private static readonly GUIContent PivotContent = new("P", "Prefab pivot. Create moves the shape here.");

        [SerializeField] private string _shapeName = string.Empty;
        [SerializeField] private List<Vector2Int> _cells = new();
        [SerializeField] private string _colorName = string.Empty;
        [SerializeField] private Color _color = Color.white;

        private BlockShapePreview _preview;

        [MenuItem("ColorBlock/Shape Creator")]
        private static void Open()
        {
            GetWindow<ShapeCreator>("Shape Creator");
        }

        private void OnEnable()
        {
            _preview = new BlockShapePreview();
        }

        private void OnDisable()
        {
            _preview.Dispose();
        }

        private void OnGUI()
        {
            DrawShapeCreator();
            GUILayout.Space(SectionSpacing);
            DrawColorCreator();
        }

        private void DrawShapeCreator()
        {
            EditorGUILayout.LabelField("Shape", EditorStyles.boldLabel);
            _shapeName = EditorGUILayout.TextField("Name", _shapeName);
            var shapeName = _shapeName.Trim();

            EditorGUILayout.Space();
            DrawGrid();
            EditorGUILayout.HelpBox(GridHint, MessageType.Info);
            EditorGUILayout.Space();
            _preview.Draw(_cells, PreviewSize);
            EditorGUILayout.Space();

            var error = Validate(shapeName);
            if (error != null)
                EditorGUILayout.HelpBox(error, MessageType.Warning);

            using (new EditorGUI.DisabledScope(error != null))
            {
                if (GUILayout.Button("Create Shape"))
                    Create(shapeName);
            }
        }

        private void DrawColorCreator()
        {
            EditorGUILayout.LabelField("Color", EditorStyles.boldLabel);
            _colorName = EditorGUILayout.TextField("Name", _colorName);
            _color = EditorGUILayout.ColorField(new GUIContent("Color"), _color, true, false, false);
            var colorName = _colorName.Trim();

            var error = ValidateName(colorName, BlockColorFactory.Exists(colorName), "color");
            if (error != null)
                EditorGUILayout.HelpBox(error, MessageType.Warning);

            using (new EditorGUI.DisabledScope(error != null))
            {
                if (GUILayout.Button("Create Color"))
                    CreateColor(colorName);
            }
        }

        private void DrawGrid()
        {
            var defaultColor = GUI.backgroundColor;
            for (var y = GridSize - 1; y >= 0; y--)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (var x = 0; x < GridSize; x++)
                    {
                        var cell = new Vector2Int(x, y);
                        var content = cell == Vector2Int.zero ? PivotContent : GUIContent.none;
                        GUI.backgroundColor = _cells.Contains(cell) ? FilledColor : defaultColor;
                        var rect = GUILayoutUtility.GetRect(content, GUI.skin.button, GUILayout.Width(CellButtonSize), GUILayout.Height(CellButtonSize));
                        var current = Event.current;
                        if (current.type == EventType.MouseDown && current.button == 1 && rect.Contains(current.mousePosition))
                        {
                            Remove(cell);
                            current.Use();
                        }
                        else if (GUI.Button(rect, content))
                        {
                            Toggle(cell);
                        }
                    }
                }
            }

            GUI.backgroundColor = defaultColor;
        }

        private void Toggle(Vector2Int cell)
        {
            Undo.RecordObject(this, "Toggle Shape Cell");
            if (!_cells.Remove(cell))
                _cells.Add(cell);
        }

        private void Remove(Vector2Int cell)
        {
            Undo.RecordObject(this, "Remove Shape Cell");
            _cells.Remove(cell);
        }

        private string Validate(string shapeName)
        {
            var error = ValidateName(shapeName, BlockShapeFactory.Exists(shapeName), "shape");
            return error ?? (_cells.Count == 0 ? "Select at least one cell." : null);
        }

        private static string ValidateName(string assetName, bool exists, string kind)
        {
            if (assetName.Length == 0)
                return "Enter a name.";

            if (assetName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return "Name contains invalid characters.";

            return exists ? $"A {kind} named {assetName} already exists." : null;
        }

        private void Create(string shapeName)
        {
            var shape = BlockShapeFactory.Create(shapeName, _cells);
            Selection.activeObject = shape;
            EditorGUIUtility.PingObject(shape);

            _shapeName = string.Empty;
            _cells.Clear();
        }

        private void CreateColor(string colorName)
        {
            var color = BlockColorFactory.Create(colorName, _color);
            Selection.activeObject = color;
            EditorGUIUtility.PingObject(color);

            _colorName = string.Empty;
        }
    }
}
