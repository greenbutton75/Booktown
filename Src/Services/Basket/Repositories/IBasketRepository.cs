using Basket.Models;

namespace Basket.Repositories;

public interface IBasketRepository
{
    Task<BasketItem> GetItemAsync(BasketItem item);
    Task UpdateItemAsync(BasketItem item);
}

