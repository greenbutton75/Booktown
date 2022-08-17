# Inventory

DB Redis

API controller
InventoryLoad (from external source)

Receive Events

InventorySpendEvent (from Saga orchestrator (Orders))

Send Events

InventoryInStockEvent (to Catalog)
InventoryOutOdStockEvent (to Catalog)


Add
	HC (+ Redis)
	Logging

Data Seed

Redis prefixes

Async method names postfix

More logging