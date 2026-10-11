using System.Collections.Generic;
using System.Linq;
using Game.BlockSystem;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Game.LevelSystem
{
    internal class LevelPaletteSync : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var shapes = Load<BlockShapeData>(importedAssets);
            var colors = Load<BlockColorData>(importedAssets);
            if (shapes.Count == 0 && colors.Count == 0 && deletedAssets.Length == 0)
                return;

            var guids = AssetDatabase.FindAssets($"t:{nameof(LevelPalette)}");
            if (guids.Length == 0)
                return;

            var palette = AssetDatabase.LoadAssetAtPath<LevelPalette>(AssetDatabase.GUIDToAssetPath(guids[0]));
            var serializedPalette = new SerializedObject(palette);
            Sync(serializedPalette.FindProperty("_shapes"), shapes);
            Sync(serializedPalette.FindProperty("_colors"), colors);

            if (serializedPalette.ApplyModifiedPropertiesWithoutUndo())
                AssetDatabase.SaveAssetIfDirty(palette);
        }

        private static List<T> Load<T>(IEnumerable<string> paths) where T : Object
        {
            return paths.Where(path => AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(T))
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToList();
        }

        private static void Sync<T>(SerializedProperty list, List<T> added) where T : Object
        {
            for (var i = list.arraySize - 1; i >= 0; i--)
            {
                var item = list.GetArrayElementAtIndex(i).objectReferenceValue;
                if (item == null)
                    list.DeleteArrayElementAtIndex(i);
                else
                    added.Remove((T)item);
            }

            foreach (var item in added)
            {
                list.arraySize++;
                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = item;
            }
        }
    }
}
