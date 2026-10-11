using System;
using Game.Core.Services;
using UnityEngine;

namespace Game.PlayerSystem
{
    internal class LivesController : BaseService, ILives
    {
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private PlayerDataStore _store;

        public int Count
        {
            get
            {
                Refill(_store.Data, DateTime.UtcNow, _config);
                return _store.Data.Lives;
            }
        }

        public bool IsFull => Count >= _config.MaxLives;

        public TimeSpan TimeToNextLife
        {
            get
            {
                var now = DateTime.UtcNow;
                Refill(_store.Data, now, _config);
                return TimeToNext(_store.Data, now, _config);
            }
        }

        protected override void BindServices()
        {
            ServiceLocator.Bind<ILives>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<ILives>(this);
        }

        public void LoseLife()
        {
            var now = DateTime.UtcNow;
            Refill(_store.Data, now, _config);
            LoseLife(_store.Data, now, _config);
            _store.Save();
        }

        public void Add(int amount)
        {
            var now = DateTime.UtcNow;
            Refill(_store.Data, now, _config);
            Add(_store.Data, amount, _config);
            _store.Save();
        }

        public void Fill()
        {
            Add(_config.MaxLives);
        }

        internal static void Refill(PlayerData data, DateTime now, PlayerConfig config)
        {
            if (data.Lives >= config.MaxLives)
                return;

            var interval = config.LifeRefillInterval.Ticks;
            var refilled = (now.Ticks - data.LifeRefillStartTicks) / interval;
            if (refilled <= 0)
                return;

            data.Lives = (int)Math.Min(config.MaxLives, data.Lives + refilled);
            data.LifeRefillStartTicks += refilled * interval;
        }

        internal static TimeSpan TimeToNext(PlayerData data, DateTime now, PlayerConfig config)
        {
            return config.LifeRefillInterval - TimeSpan.FromTicks(now.Ticks - data.LifeRefillStartTicks);
        }

        internal static void Add(PlayerData data, int amount, PlayerConfig config)
        {
            data.Lives = Math.Min(config.MaxLives, data.Lives + amount);
        }

        internal static void LoseLife(PlayerData data, DateTime now, PlayerConfig config)
        {
            if (data.Lives <= 0)
                return;

            if (data.Lives >= config.MaxLives)
                data.LifeRefillStartTicks = now.Ticks;

            data.Lives--;
        }
    }
}
