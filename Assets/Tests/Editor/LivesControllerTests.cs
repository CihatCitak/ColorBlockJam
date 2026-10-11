using System;
using Game.PlayerSystem;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Tests
{
    public class LivesControllerTests
    {
        private const int MaxLives = 5;

        private static readonly DateTime Start = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        private PlayerConfig _config;

        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<PlayerConfig>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void LoseLife_FromFull_RefillsAfterOneInterval()
        {
            var data = new PlayerData { Lives = MaxLives };

            LivesController.LoseLife(data, Start, _config);
            LivesController.Refill(data, Start + _config.LifeRefillInterval - TimeSpan.FromSeconds(1), _config);
            Assert.AreEqual(MaxLives - 1, data.Lives);

            LivesController.Refill(data, Start + _config.LifeRefillInterval, _config);
            Assert.AreEqual(MaxLives, data.Lives);
        }

        [Test]
        public void Refill_LongTimeAway_CapsAtMax()
        {
            var data = new PlayerData { Lives = 1, LifeRefillStartTicks = Start.Ticks };

            LivesController.Refill(data, Start.AddDays(1), _config);

            Assert.AreEqual(MaxLives, data.Lives);
        }

        [Test]
        public void Refill_KeepsElapsedTimeTowardNextLife()
        {
            var data = new PlayerData { Lives = 2, LifeRefillStartTicks = Start.Ticks };
            var now = Start + _config.LifeRefillInterval * 1.5;

            LivesController.Refill(data, now, _config);

            Assert.AreEqual(3, data.Lives);
            Assert.AreEqual(_config.LifeRefillInterval * 0.5, LivesController.TimeToNext(data, now, _config));
        }

        [Test]
        public void LoseLife_WhenNotFull_KeepsRunningTimer()
        {
            var data = new PlayerData { Lives = 3, LifeRefillStartTicks = Start.Ticks };

            LivesController.LoseLife(data, Start.AddMinutes(10), _config);

            Assert.AreEqual(2, data.Lives);
            Assert.AreEqual(Start.Ticks, data.LifeRefillStartTicks);
        }

        [Test]
        public void LoseLife_AtZero_StaysZero()
        {
            var data = new PlayerData { Lives = 0 };

            LivesController.LoseLife(data, Start, _config);

            Assert.AreEqual(0, data.Lives);
        }
    }
}
