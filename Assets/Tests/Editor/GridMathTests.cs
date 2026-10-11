using Game.GridSystem;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class GridMathTests
    {
        [Test]
        public void CellToLocal_CentersGridOnOrigin()
        {
            var size = new Vector2Int(4, 5);

            Assert.AreEqual(new Vector2(-1.5f, -2f), GridMath.CellToLocal(new Vector2Int(0, 0), size, 1f));
            Assert.AreEqual(new Vector2(3f, 4f), GridMath.CellToLocal(new Vector2Int(3, 4), size, 2f));
        }

        [TestCase(4, 5)]
        [TestCase(3, 3)]
        public void LocalToCell_ReturnsCellOfEveryCellCenter(int width, int height)
        {
            var size = new Vector2Int(width, height);

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var cell = new Vector2Int(x, y);
                Assert.AreEqual(cell, GridMath.LocalToCell(GridMath.CellToLocal(cell, size, 1.5f), size, 1.5f));
            }
        }
    }
}
