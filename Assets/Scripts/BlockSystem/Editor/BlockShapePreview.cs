using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.BlockSystem
{
    internal class BlockShapePreview
    {
        public const int GridSize = 5;

        private const float CameraDistance = 10f;
        private const float PrefabMargin = 1.1f;

        private readonly PreviewRenderUtility _preview = new();
        private readonly List<Vector2Int> _cells = new();
        private readonly float _cellSize;
        private GameObject _instance;
        private GameObject _prefab;

        public BlockShapePreview()
        {
            _cellSize = BlockShapeFactory.CellSize;
            var camera = _preview.camera;
            camera.orthographic = true;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = CameraDistance * 2f;
            camera.transform.rotation = Quaternion.identity;
            _preview.lights[0].transform.rotation = Quaternion.Euler(20f, -20f, 0f);
        }

        public void Draw(IReadOnlyList<Vector2Int> cells, float size)
        {
            if (!cells.SequenceEqual(_cells))
                Rebuild(cells);

            var rect = GUILayoutUtility.GetRect(size, size, GUILayout.ExpandWidth(false));
            if (Event.current.type != EventType.Repaint)
                return;

            var center = Vector2.one * ((GridSize - 1) * 0.5f * _cellSize);
            Render(rect, GUIStyle.none, center, GridSize * _cellSize * 0.5f);
        }

        public void Draw(GameObject prefab, Rect rect, GUIStyle background)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (_prefab != prefab)
            {
                _prefab = prefab;
                Replace(_preview.InstantiatePrefabInScene(prefab));
            }

            var bounds = new Bounds(_instance.transform.position, Vector3.zero);
            foreach (var partRenderer in _instance.GetComponentsInChildren<Renderer>())
                bounds.Encapsulate(partRenderer.bounds);

            var halfHeight = Mathf.Max(bounds.extents.y, bounds.extents.x * rect.height / rect.width) * PrefabMargin;
            Render(rect, background, bounds.center, halfHeight);
        }

        public void Dispose()
        {
            _preview.Cleanup();
        }

        private void Rebuild(IReadOnlyList<Vector2Int> cells)
        {
            _cells.Clear();
            _cells.AddRange(cells);

            if (_cells.Count == 0)
            {
                Replace(null);
                return;
            }

            var scene = EditorSceneManager.NewPreviewScene();
            var instance = BlockShapeFactory.Build(_cells, scene);
            _preview.AddSingleGO(instance);
            EditorSceneManager.ClosePreviewScene(scene);
            Replace(instance);
        }

        private void Replace(GameObject instance)
        {
            if (_instance != null)
                Object.DestroyImmediate(_instance);

            _instance = instance;
        }

        private void Render(Rect rect, GUIStyle background, Vector2 center, float halfHeight)
        {
            var camera = _preview.camera;
            camera.orthographicSize = halfHeight;
            camera.transform.position = new Vector3(center.x, center.y, -CameraDistance);

            _preview.BeginPreview(rect, background);
            camera.Render();
            _preview.EndAndDrawPreview(rect);
        }
    }
}
