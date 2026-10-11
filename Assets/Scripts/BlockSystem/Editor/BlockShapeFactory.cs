using System.Collections.Generic;
using System.Linq;
using Game.GridSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.BlockSystem
{
    internal static class BlockShapeFactory
    {
        private const string ShapeFolder = "Assets/ScriptableObjects/Blocks/Shapes";
        private const string PrefabFolder = "Assets/Prefabs/BlockSystem/Blocks";
        private const string ShapePrefix = "BlockShape_";
        private const string BlockPrefix = "Block_";

        public static float CellSize => LoadSingle<GridConfig>().CellSize;

        public static bool Exists(string name)
        {
            return AssetDatabase.LoadMainAssetAtPath(ShapePath(name)) != null || AssetDatabase.LoadMainAssetAtPath(PrefabPath(name)) != null;
        }

        public static BlockShapeData Create(string name, IReadOnlyList<Vector2Int> cells)
        {
            var aligned = AlignToPivot(cells);
            var prefab = SavePrefab(PrefabPath(name), aligned);

            var shape = ScriptableObject.CreateInstance<BlockShapeData>();
            AssetDatabase.CreateAsset(shape, ShapePath(name));

            var serializedShape = new SerializedObject(shape);
            var cellsProperty = serializedShape.FindProperty("_cells");
            cellsProperty.arraySize = aligned.Count;
            for (var i = 0; i < aligned.Count; i++)
                cellsProperty.GetArrayElementAtIndex(i).vector2IntValue = aligned[i];

            serializedShape.FindProperty("_prefab").objectReferenceValue = prefab;
            serializedShape.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            return shape;
        }

        public static GameObject Build(IReadOnlyList<Vector2Int> cells, Scene scene)
        {
            var config = LoadSingle<BlockAuthoringConfig>();
            var cellSize = CellSize;
            var root = (GameObject)PrefabUtility.InstantiatePrefab(config.BlockPrefab.gameObject, scene);

            foreach (var cell in cells)
            {
                var cellCollider = (GameObject)PrefabUtility.InstantiatePrefab(config.CellColliderPrefab, root.transform);
                cellCollider.transform.localPosition = (Vector2)cell * cellSize;
            }

            foreach (var part in BlockPartLayout.Build(cells, cellSize))
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(config.GetPartPrefab(part.Type), root.transform);
                instance.transform.localPosition = part.Position;
                instance.transform.localRotation = Quaternion.Euler(0f, 0f, part.Rotation);
            }

            AssignReferences(root.GetComponent<Block>(), config);
            return root;
        }

        private static Block SavePrefab(string path, IReadOnlyList<Vector2Int> cells)
        {
            var scene = EditorSceneManager.NewPreviewScene();
            try
            {
                return PrefabUtility.SaveAsPrefabAsset(Build(cells, scene), path).GetComponent<Block>();
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(scene);
            }
        }

        private static List<Vector2Int> AlignToPivot(IReadOnlyList<Vector2Int> cells)
        {
            var min = cells.Aggregate(Vector2Int.Min);
            return cells.Select(cell => cell - min).ToList();
        }

        private static void AssignReferences(Block block, BlockAuthoringConfig config)
        {
            var renderers = block.GetComponentsInChildren<Renderer>();
            var baseMaterial = renderers[0].sharedMaterial;

            var serializedBlock = new SerializedObject(block);
            SetArray(serializedBlock.FindProperty("_cellColliders"), block.GetComponentsInChildren<Collider2D>());
            SetArray(serializedBlock.FindProperty("_renderers"), renderers);
            SetArray(serializedBlock.FindProperty("_plainMaterials"), new Object[] { baseMaterial });
            SetArray(serializedBlock.FindProperty("_outlinedMaterials"), new Object[] { baseMaterial, config.OutlineMaskMaterial, config.OutlineFillMaterial });
            serializedBlock.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetArray(SerializedProperty property, Object[] values)
        {
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        private static string ShapePath(string name)
        {
            return $"{ShapeFolder}/{ShapePrefix}{name}.asset";
        }

        private static string PrefabPath(string name)
        {
            return $"{PrefabFolder}/{BlockPrefix}{name}.prefab";
        }

        public static T LoadSingle<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
