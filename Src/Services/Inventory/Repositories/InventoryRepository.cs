using Infrastructure.Core.Exceptions;
using Infrastructure.Core.Extensions;
using Inventory.Models;
using StackExchange.Redis;

namespace Inventory.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ILogger<InventoryRepository> _logger;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly string keyPrefix = "inventory:";

    public InventoryRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
    {
        _logger = loggerFactory.CreateLogger<InventoryRepository>();
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public async Task UpdateItemAsync(InventoryItem item)
    {
        await _database.StringSetAsync(keyPrefix + item.ProductId, item.Quantity);
    }

    public async Task<InventoryItem> GetItemAsync(InventoryItem item)
    {
        var data = await _database.StringGetAsync(keyPrefix + item.ProductId);

        if (data.IsNullOrEmpty)
        {
            return null;
        }
        return new InventoryItem { ProductId = item.ProductId.RemovePrefix(keyPrefix), Quantity = Int32.Parse(data) };
    }
}
