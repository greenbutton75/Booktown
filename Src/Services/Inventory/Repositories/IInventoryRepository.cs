using Inventory.Models;

namespace Inventory.Repositories;

public interface IInventoryRepository
{
    Task<IEnumerable<InventoryItem>> GetAll();
    Task DoNothing();
}

