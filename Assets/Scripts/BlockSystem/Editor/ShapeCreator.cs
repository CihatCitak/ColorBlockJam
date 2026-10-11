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

        private static readonly Color FilledColor = new(0.3f, 0.8f, 0.4f);
        private static readonly GUIContent PivotContent = new("P", "Prefab pivot. Create moves the shape here.");

        [SerializeField] private string _shapeName = string.Empty;
        [SerializeField] private List<Vector2Int> _cells = new();

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
            _shapeName = EditorGUILayout.TextField("Name", _shapeName);
            var shapeName = _shapeName.Trim();

            EditorGUILayout.Space();
            DrawGrid();
            EditorGUILayout.Space();
            _preview.Draw(_cells, PreviewSize);
            EditorGUILayout.Space();

            var error = Validate(shapeName);
            if (error != null)
                EditorGUILayout.HelpBox(error, MessageType.Warning);

            using (new EditorGUI.DisabledScope(error != null))
            {
                if (GUILayout.Button("Create"))
                    Create(shapeName);
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
                        if (GUILayout.Button(content, GUILayout.Width(CellButtonSize), GUILayout.Height(CellButtonSize)))
                            Toggle(cell);
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

        private string Validate(string shapeName)
        {
            if (shapeName.Length == 0)
                return "Enter a name.";

            if (shapeName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return "Name contains invalid characters.";

            if (BlockShapeFactory.Exists(shapeName))
                return $"A shape named {shapeName} already exists.";

            return _cells.Count == 0 ? "Select at least one cell." : null;
        }

        private void Create(string shapeName)
        {
            var shape = BlockShapeFactory.Create(shapeName, _cells);
            Selection.activeObject = shape;
            EditorGUIUtility.PingObject(shape);

            _shapeName = string.Empty;
            _cells.Clear();
        }
    }
}
