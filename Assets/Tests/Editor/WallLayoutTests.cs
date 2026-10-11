using System.Linq;
using Game.GridSystem;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class WallLayoutTests
    {
        private static readonly GridEdge[] NoOpenings = { };

        [Test]
        public void Build_SingleCell_IsFourLCorners()
        {
            var pieces = WallLayout.Build(Vector2Int.one, cell => cell == Vector2Int.zero, NoOpenings, 1f);

            Assert.AreEqual(4, pieces.Count);
            Assert.IsTrue(pieces.All(p => p.Type == WallPieceType.LCorner));
        }

        [Test]
        public void Build_HorizontalBoard_TurnsTopAndBottomWalls()
        {
            var size = new Vector2Int(2, 1);

            var pieces = WallLayout.Build(size, cell => cell.y == 0 && cell.x >= 0 && cell.x < 2, NoOpenings, 1f);

            var walls = pieces.Where(p => p.Type == WallPieceType.Wall).ToList();
            Assert.AreEqual(4, walls.Count);
            Assert.IsTrue(walls.All(p => Mathf.Approximately(p.Rotation, 90f)));
        }

        [Test]
        public void Build_BoardWithHole_FillsTheHoleWithCorners()
        {
            var size = new Vector2Int(3, 3);
            var hole = new Vector2Int(1, 1);

            var pieces = WallLayout.Build(size, cell => cell.x >= 0 && cell.y >= 0 && cell.x < 3 && cell.y < 3 && cell != hole, NoOpenings, 1f);

            Assert.AreEqual(4, pieces.Count(p => p.Type == WallPieceType.LCorner));
            Assert.AreEqual(4, pieces.Count(p => p.Type == WallPieceType.Corner));
            Assert.AreEqual(16, pieces.Count(p => p.Type == WallPieceType.Wall));
        }

        [Test]
        public void Build_Opening_LeavesNoWallInIt()
        {
            var openings = new[] { new GridEdge(Vector2Int.zero, Vector2Int.up) };

            var pieces = WallLayout.Build(Vector2Int.one, cell => cell == Vector2Int.zero, openings, 1f);

            Assert.IsFalse(pieces.Any(p => p.Position.y > 0.5f && Mathf.Abs(p.Position.x) < 0.5f));
            Assert.AreEqual(2, pieces.Count(p => p.Type == WallPieceType.Corner));
            Assert.AreEqual(2, pieces.Count(p => p.Type == WallPieceType.LCorner));
        }
    }
}
