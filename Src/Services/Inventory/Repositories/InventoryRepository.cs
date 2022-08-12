using Inventory.Models;
using StackExchange.Redis;

namespace Inventory.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ILogger<InventoryRepository> _logger;
    //private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public InventoryRepository(ILoggerFactory loggerFactory) //, ConnectionMultiplexer redis
    {
        _logger = loggerFactory.CreateLogger<InventoryRepository>();
        //_redis = redis;
        //_database = redis.GetDatabase();
    }
/*
    public async Task<bool> DeleteBasketAsync(string id)
    {
        return await _database.KeyDeleteAsync(id);
    }
*/
    public async Task DoNothing()
    {
       await Task.CompletedTask; // TODO
    }

    public async Task<IEnumerable<InventoryItem>> GetAll()
    {
        var server = GetServer();
        var data = server.Keys();

        var items = new List<InventoryItem>();
        foreach (var item in data)
        {
            items.Add( new InventoryItem { ProductId = item.ToString(), Quantity = Int32.Parse(await _database.StringGetAsync(item)) });
        }

        return items;
        /*
        if (!created)
        {
            _logger.LogInformation("Problem occur persisting the item.");
            return null;
        }
        */
    }

    private IServer GetServer()
    {
        throw new NotImplementedException();
        // var endpoint = _redis.GetEndPoints();
        // return _redis.GetServer(endpoint.First());
    }
}
