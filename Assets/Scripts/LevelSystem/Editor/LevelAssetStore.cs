using System.Collections.Generic;
using Game.BlockSystem;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Game.LevelSystem
{
    internal static class LevelAssetStore
    {
        private const string LevelFolder = "Assets/ScriptableObjects/Levels";
        private const string GroupName = "Levels";

        private static LevelCatalog CachedCatalog;

        public static int Count => Catalog.Levels.Count;

        private static LevelCatalog Catalog => CachedCatalog != null ? CachedCatalog : CachedCatalog = BlockShapeFactory.LoadSingle<LevelCatalog>();

        public static LevelData Find(int number)
        {
            var levels = Catalog.Levels;
            return number >= 1 && number <= levels.Count ? levels[number - 1].editorAsset : null;
        }

        public static void Save(int number, LevelDraft draft)
        {
            var level = Find(number);
            if (level == null)
                level = Create(number);

            Write(level, draft);
            AssetDatabase.SaveAssets();
        }

        private static LevelData Create(int number)
        {
            var name = $"Level_{number:000}";
            var path = $"{LevelFolder}/{name}.asset";
            var level = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(level, path);

            var guid = AssetDatabase.AssetPathToGUID(path);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            settings.CreateOrMoveEntry(guid, settings.FindGroup(GroupName)).address = name;

            var serializedCatalog = new SerializedObject(Catalog);
            var levels = serializedCatalog.FindProperty("_levels");
            levels.arraySize++;
            levels.GetArrayElementAtIndex(levels.arraySize - 1).FindPropertyRelative("m_AssetGUID").stringValue = guid;
            serializedCatalog.ApplyModifiedPropertiesWithoutUndo();
            return level;
        }

        private static void Write(LevelData level, LevelDraft draft)
        {
            var serializedLevel = new SerializedObject(level);
            serializedLevel.FindProperty("_gridSize").vector2IntValue = draft.Size;
            serializedLevel.FindProperty("_timeLimit").intValue = draft.TimeLimit;
            WriteCells(serializedLevel.FindProperty("_holes"), draft.Holes);
            WriteCells(serializedLevel.FindProperty("_obstacles"), draft.Walls);

            var doors = new List<DraftDoor>();
            draft.CollectDoors(doors);
            var exits = serializedLevel.FindProperty("_exits");
            exits.arraySize = doors.Count;
            for (var i = 0; i < doors.Count; i++)
            {
                var exit = exits.GetArrayElementAtIndex(i);
                exit.FindPropertyRelative("_cell").vector2IntValue = doors[i].Cell;
                exit.FindPropertyRelative("_side").enumValueIndex = (int)doors[i].Side;
                exit.FindPropertyRelative("_length").intValue = doors[i].Length;
                exit.FindPropertyRelative("_color").objectReferenceValue = doors[i].Color;
            }

            var blocks = serializedLevel.FindProperty("_blocks");
            blocks.arraySize = draft.Blocks.Count;
            for (var i = 0; i < draft.Blocks.Count; i++)
            {
                var block = blocks.GetArrayElementAtIndex(i);
                block.FindPropertyRelative("_shape").objectReferenceValue = draft.Blocks[i].Shape;
                block.FindPropertyRelative("_color").objectReferenceValue = draft.Blocks[i].Color;
                block.FindPropertyRelative("_cell").vector2IntValue = draft.Blocks[i].Cell;
                block.FindPropertyRelative("_quarterTurns").intValue = draft.Blocks[i].QuarterTurns;
            }

            serializedLevel.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WriteCells(SerializedProperty property, IReadOnlyList<Vector2Int> cells)
        {
            property.arraySize = cells.Count;
            for (var i = 0; i < cells.Count; i++)
                property.GetArrayElementAtIndex(i).vector2IntValue = cells[i];
        }
    }
}
