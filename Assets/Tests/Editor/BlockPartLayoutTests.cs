using System.Linq;
using Game.BlockSystem;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class BlockPartLayoutTests
    {
        [Test]
        public void Build_SingleCell_IsFourOuterCorners()
        {
            var parts = BlockPartLayout.Build(new[] { Vector2Int.zero }, 1f);

            Assert.AreEqual(4, parts.Count);
            Assert.IsTrue(parts.All(p => p.Type == BlockPartType.OuterCorner));
        }

        [Test]
        public void Build_TwoCells_JoinsThemWithEdges()
        {
            var parts = BlockPartLayout.Build(new[] { Vector2Int.zero, Vector2Int.right }, 1f);

            Assert.AreEqual(4, parts.Count(p => p.Type == BlockPartType.OuterCorner));
            Assert.AreEqual(4, parts.Count(p => p.Type == BlockPartType.Edge));
        }

        [Test]
        public void Build_LShape_UsesOneInnerCornerForTheConcaveVertex()
        {
            var parts = BlockPartLayout.Build(new[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up }, 1f);

            Assert.AreEqual(1, parts.Count(p => p.Type == BlockPartType.InnerCorner));
            Assert.AreEqual(10, parts.Count);
        }
    }
}
