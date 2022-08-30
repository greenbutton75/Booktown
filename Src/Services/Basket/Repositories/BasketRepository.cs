using Infrastructure.Core.Exceptions;
using Infrastructure.Core.Extensions;
using Basket.Models;
using StackExchange.Redis;

namespace Basket.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly ILogger<BasketRepository> _logger;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly string keyPrefix = "basket:";

    public BasketRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
    {
        _logger = loggerFactory.CreateLogger<BasketRepository>();
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public async Task UpdateItemAsync(BasketItem item)
    {
        await _database.StringSetAsync(keyPrefix + item.ProductId, item.Quantity);
    }

    public async Task<BasketItem> GetItemAsync(BasketItem item)
    {
        var data = await _database.StringGetAsync(keyPrefix + item.ProductId);

        if (data.IsNullOrEmpty)
        {
            return null;
        }
        return new BasketItem { ProductId = item.ProductId.RemovePrefix(keyPrefix), Quantity = Int32.Parse(data) };
    }
}
