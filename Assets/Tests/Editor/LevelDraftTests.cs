using System.Collections.Generic;
using Game.BlockSystem;
using Game.ExitSystem;
using Game.LevelSystem;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests
{
    public class LevelDraftTests
    {
        private BlockShapeData _lShape;
        private BlockColorData _red;
        private BlockColorData _blue;

        [SetUp]
        public void SetUp()
        {
            _lShape = ScriptableObject.CreateInstance<BlockShapeData>();
            var cells = new SerializedObject(_lShape).FindProperty("_cells");
            Vector2Int[] lCells = { new(0, 0), new(0, 1), new(0, 2), new(1, 0) };
            cells.arraySize = lCells.Length;
            for (var i = 0; i < lCells.Length; i++)
                cells.GetArrayElementAtIndex(i).vector2IntValue = lCells[i];
            cells.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            _red = ScriptableObject.CreateInstance<BlockColorData>();
            _blue = ScriptableObject.CreateInstance<BlockColorData>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_lShape);
            Object.DestroyImmediate(_red);
            Object.DestroyImmediate(_blue);
        }

        [Test]
        public void Footprint_QuarterTurn_StartsAtPlacementCell()
        {
            var footprint = new List<Vector2Int>();

            LevelDraft.Footprint(new DraftBlock(_lShape, _red, new Vector2Int(2, 1), 1), footprint);

            CollectionAssert.AreEquivalent(new[] { new Vector2Int(2, 1), new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(4, 2) }, footprint);
        }

        [Test]
        public void TryAddBlock_OverlappingBlock_IsRejected()
        {
            var draft = new LevelDraft();
            draft.TryAddBlock(new DraftBlock(_lShape, _red, Vector2Int.zero, 0));

            var added = draft.TryAddBlock(new DraftBlock(_lShape, _blue, new Vector2Int(1, 0), 0));

            Assert.IsFalse(added);
            Assert.AreEqual(1, draft.Blocks.Count);
        }

        [Test]
        public void CollectDoors_AdjacentEdges_MergeOnlyWhenSameColor()
        {
            var draft = new LevelDraft();
            draft.SetDoor(new Vector2Int(0, 4), ExitSide.Up, _red);
            draft.SetDoor(new Vector2Int(1, 4), ExitSide.Up, _red);
            draft.SetDoor(new Vector2Int(2, 4), ExitSide.Up, _blue);
            var doors = new List<DraftDoor>();

            draft.CollectDoors(doors);

            Assert.AreEqual(2, doors.Count);
            Assert.AreEqual((new Vector2Int(0, 4), 2, _red), (doors[0].Cell, doors[0].Length, doors[0].Color));
            Assert.AreEqual((new Vector2Int(2, 4), 1, _blue), (doors[1].Cell, doors[1].Length, doors[1].Color));
        }

        [Test]
        public void SetDoor_InsideTheBoard_IsIgnored()
        {
            var draft = new LevelDraft();
            var doors = new List<DraftDoor>();

            draft.SetDoor(new Vector2Int(2, 2), ExitSide.Up, _red);
            draft.CollectDoors(doors);

            Assert.IsEmpty(doors);
        }

        [Test]
        public void SetDoor_FacingAHole_IsRemovedWhenTheHoleIsFilled()
        {
            var draft = new LevelDraft();
            var cell = new Vector2Int(2, 2);
            draft.AddHole(cell + Vector2Int.up);

            draft.SetDoor(cell, ExitSide.Up, _red);
            Assert.IsTrue(draft.HasDoor(cell, ExitSide.Up));

            draft.RemoveHole(cell + Vector2Int.up);
            Assert.IsFalse(draft.HasDoor(cell, ExitSide.Up));
        }
    }
}
