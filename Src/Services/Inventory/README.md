# Inventory

DB Redis

API controller
InventoryLoad (from external source)

Receive Events

InventorySpendEvent (from Saga orchestrator (Orders))

Send Events

InventoryInStockEvent (to Catalog)
InventoryOutOfStockEvent (to Catalog)


