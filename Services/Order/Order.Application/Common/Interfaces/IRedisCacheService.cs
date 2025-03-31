using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Common.Interfaces;

public interface IRedisCacheService
{
    Task<Response<bool>> DeleteAsync(string key);
    Task<Response<T>> GetAsync<T>(string Id);
    Task<Response<bool>> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
}
