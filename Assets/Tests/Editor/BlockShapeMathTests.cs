using System.Collections.Generic;
using Game.BlockSystem;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class BlockShapeMathTests
    {
        private static readonly Vector2Int[] LShape = { new(0, 0), new(0, 1), new(0, 2), new(1, 0) };

        [Test]
        public void RotationOffset_QuarterTurnClockwise_KeepsShapeAboveOrigin()
        {
            Assert.AreEqual(new Vector2Int(0, 1), BlockShapeMath.RotationOffset(LShape, 1));
        }

        [Test]
        public void RotationOffset_FullTurn_IsZero()
        {
            Assert.AreEqual(Vector2Int.zero, BlockShapeMath.RotationOffset(LShape, 4));
        }

        [Test]
        public void ExitPath_LShapeTowardItsNotch_CoversTheNotch()
        {
            var path = new List<Vector2Int>();

            BlockShapeMath.ExitPath(LShape, Vector2Int.up, path);

            CollectionAssert.AreEquivalent(new[] { new Vector2Int(1, 1), new Vector2Int(1, 2) }, path);
        }

        [Test]
        public void ExitPath_AlongTheBlock_IsEmpty()
        {
            var path = new List<Vector2Int>();

            BlockShapeMath.ExitPath(LShape, Vector2Int.left, path);

            Assert.IsEmpty(path);
        }
    }
}
