using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IRedisService
    {
        Task SetAsync(string key, string value, TimeSpan expiry);

        Task<string?> GetAsync(string key);

        Task RemoveAsync(string key);
    }
}
