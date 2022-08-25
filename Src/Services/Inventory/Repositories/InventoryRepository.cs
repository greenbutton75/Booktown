using Infrastructure.Core.Exceptions;
using Inventory.Models;
using StackExchange.Redis;

namespace Inventory.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ILogger<InventoryRepository> _logger;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public InventoryRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
    {
        _logger = loggerFactory.CreateLogger<InventoryRepository>();
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public async Task UpdateItemAsync(InventoryItem item)
    {
        await _database.StringSetAsync(item.ProductId, item.Quantity);
    }

    public async Task<InventoryItem> GetItemAsync(InventoryItem item)
    {
        var data = await _database.StringGetAsync(item.ProductId);

        if (data.IsNullOrEmpty)
        {
            return null;
        }
        return new InventoryItem { ProductId = item.ProductId, Quantity = Int32.Parse(data) };
    }
}
