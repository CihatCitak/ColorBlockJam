using Game.LevelSystem;
using NUnit.Framework;

namespace Game.Tests
{
    public class LevelLoaderTests
    {
        [Test]
        public void CatalogIndex_PastLastLevel_WrapsToFirst()
        {
            Assert.AreEqual(0, LevelLoader.CatalogIndex(1, 3));
            Assert.AreEqual(2, LevelLoader.CatalogIndex(3, 3));
            Assert.AreEqual(0, LevelLoader.CatalogIndex(4, 3));
        }
    }
}
