using Game.LevelSystem;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests
{
    public class LevelCatalogTests
    {
        [Test]
        public void GetByLevelNumber_PastLastLevel_WrapsToFirst()
        {
            var catalog = ScriptableObject.CreateInstance<LevelCatalog>();
            var serializedCatalog = new SerializedObject(catalog);
            var levels = serializedCatalog.FindProperty("_levels");
            levels.arraySize = 3;
            for (var i = 0; i < levels.arraySize; i++)
                levels.GetArrayElementAtIndex(i).FindPropertyRelative("m_AssetGUID").stringValue = $"level{i}";
            serializedCatalog.ApplyModifiedPropertiesWithoutUndo();

            Assert.AreEqual("level0", catalog.GetByLevelNumber(1).AssetGUID);
            Assert.AreEqual("level2", catalog.GetByLevelNumber(3).AssetGUID);
            Assert.AreEqual("level0", catalog.GetByLevelNumber(4).AssetGUID);

            Object.DestroyImmediate(catalog);
        }
    }
}
