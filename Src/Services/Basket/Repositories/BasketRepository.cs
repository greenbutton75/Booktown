using Infrastructure.Core.Exceptions;
using Infrastructure.Core.Extensions;
using Basket.Models;
using StackExchange.Redis;
using BasketProto;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

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

    public async Task<UserBasket> SetBasketAsync(UserBasket basket)
    {
        var key = keyPrefix + basket.Email;
        bool basketExists = await _database.KeyExistsAsync(key);
        if (!basketExists)
        {
            basket.Id = Guid.NewGuid().ToString();
            basket.Created = Timestamp.FromDateTime(DateTime.UtcNow);
        }

        var byteArray = ProtoToByteArray(basket);
        await _database.StringSetAsync(key, byteArray);

        return basket;
    }

    public async Task<UserBasket?> GetBasketAsync(string email)
    {
        var key = keyPrefix + email;
        bool basketExists = await _database.KeyExistsAsync(key);
        if (!basketExists) return  null;

        var data = await _database.StringGetAsync(key);

        var basket = UserBasket.Parser.ParseFrom (data);

        return basket;
    }
    public async Task<UserBasket?> DeleteBasketAsync(string email)
    {
        var key = keyPrefix + email;
        bool basketExists = await _database.KeyExistsAsync(key);
        if (!basketExists) return null;

        var data = await _database.StringGetAsync(key);

        var basket = UserBasket.Parser.ParseFrom(data);

        await _database.KeyDeleteAsync(key);

        return basket;
    }

    private byte[] ProtoToByteArray(IMessage message)
    {
        int size = message.CalculateSize();
        byte[] buffer = new byte[size];
        CodedOutputStream output = new CodedOutputStream(buffer);
        message.WriteTo(output);
        return buffer;
    }
}
