using Inventory.Models;

namespace Inventory.Repositories;

public interface IInventoryRepository
{
    Task<InventoryItem> GetItemAsync(InventoryItem item);
    Task UpdateItemAsync(InventoryItem item);
}

