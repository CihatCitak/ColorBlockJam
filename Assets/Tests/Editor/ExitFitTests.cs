using Game.BlockSystem;
using Game.ExitSystem;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class ExitFitTests
    {
        private static readonly Vector2Int TopExitCell = new(1, 5);
        private const int TopExitLength = 2;

        private BlockColorData _red;
        private BlockColorData _blue;

        [SetUp]
        public void SetUp()
        {
            _red = ScriptableObject.CreateInstance<BlockColorData>();
            _blue = ScriptableObject.CreateInstance<BlockColorData>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_red);
            Object.DestroyImmediate(_blue);
        }

        [Test]
        public void Fits_BlockAsWideAsDoor_IsTrue()
        {
            Assert.IsTrue(ExitFit.Fits(_red, new RectInt(1, 3, 2, 3), _red, TopExitCell, TopExitLength, Vector2Int.up));
        }

        [Test]
        public void Fits_BlockWiderThanDoor_IsFalse()
        {
            Assert.IsFalse(ExitFit.Fits(_red, new RectInt(1, 5, 3, 1), _red, TopExitCell, TopExitLength, Vector2Int.up));
        }

        [Test]
        public void Fits_BlockShiftedPastDoor_IsFalse()
        {
            Assert.IsFalse(ExitFit.Fits(_red, new RectInt(2, 5, 2, 1), _red, TopExitCell, TopExitLength, Vector2Int.up));
        }

        [Test]
        public void Fits_OtherColor_IsFalse()
        {
            Assert.IsFalse(ExitFit.Fits(_blue, new RectInt(1, 5, 2, 1), _red, TopExitCell, TopExitLength, Vector2Int.up));
        }

        [Test]
        public void Fits_LShape_DependsOnTheSideFacingTheDoor()
        {
            var lShape = new RectInt(0, 1, 2, 3);

            Assert.IsTrue(ExitFit.Fits(_red, lShape, _red, new Vector2Int(0, 3), 2, Vector2Int.up));
            Assert.IsFalse(ExitFit.Fits(_red, lShape, _red, new Vector2Int(0, 1), 2, Vector2Int.left));
            Assert.IsTrue(ExitFit.Fits(_red, lShape, _red, new Vector2Int(0, 1), 3, Vector2Int.left));
        }

        [Test]
        public void Fits_SideDoor_UsesVerticalSpan()
        {
            var leftExitCell = new Vector2Int(0, 1);

            Assert.IsTrue(ExitFit.Fits(_red, new RectInt(0, 1, 3, 2), _red, leftExitCell, 2, Vector2Int.left));
            Assert.IsFalse(ExitFit.Fits(_red, new RectInt(0, 1, 1, 3), _red, leftExitCell, 2, Vector2Int.left));
        }
    }
}
