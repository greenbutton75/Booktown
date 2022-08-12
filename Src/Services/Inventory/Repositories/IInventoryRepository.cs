using Inventory.Models;

namespace Inventory.Repositories;

public interface IInventoryRepository
{
    Task<InventoryItem> GetItem(InventoryItem item);
    Task UpdateItem(InventoryItem item);
}

