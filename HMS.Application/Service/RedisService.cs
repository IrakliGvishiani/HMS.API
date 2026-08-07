using HMS.Application.Contracts.Service;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Service
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }
        public async Task<string?> GetAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
        

        public async Task SetAsync(string key, string value, TimeSpan expiry)
        {
            await _database.StringSetAsync(key, value, expiry);
        }
    }
}
