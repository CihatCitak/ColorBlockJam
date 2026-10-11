using UnityEditor;
using UnityEngine;

namespace Game.BlockSystem
{
    internal static class BlockColorFactory
    {
        private const string ColorFolder = "Assets/ScriptableObjects/Blocks/Colors";
        private const string ColorPrefix = "BlockColor_";

        public static bool Exists(string name)
        {
            return AssetDatabase.LoadMainAssetAtPath(ColorPath(name)) != null;
        }

        public static BlockColorData Create(string name, Color color)
        {
            var colorData = ScriptableObject.CreateInstance<BlockColorData>();
            var serializedColor = new SerializedObject(colorData);
            serializedColor.FindProperty("_color").colorValue = color;
            serializedColor.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(colorData, ColorPath(name));
            AssetDatabase.SaveAssets();
            return colorData;
        }

        private static string ColorPath(string name)
        {
            return $"{ColorFolder}/{ColorPrefix}{name}.asset";
        }
    }
}
