using BasketProto;

namespace Basket.Repositories;

public interface IBasketRepository
{
    public Task<UserBasket?> GetBasketAsync(string email);
    public Task<UserBasket> SetBasketAsync(UserBasket basket);
    public Task<UserBasket?> DeleteBasketAsync(string email);
}

