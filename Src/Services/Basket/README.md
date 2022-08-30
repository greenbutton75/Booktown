# Basket service

This is a very simple service - we need to store and retrieve user basket from database fast. Very fast.
So we'll use `Redis` as a storage and `protobuf` Serializing instead of JSON Serializing/Deserializing.

You can find performance and memory footprint comparisons here ([Protobuf In C# .NET – Part 4 – Performance Comparisons](https://dotnetcoretutorials.com/2022/01/18/protobuf-in-c-net-part-4-performance-comparisons/))

![Architecture](../../../img/Basket-architecture.png)

## gRPC Calls

GetBasket (from BFF)
DeleteBasket (from BFF)
PutBasket (from BFF)

## Persistent storage

Since we need to store very simple key-value pairs - `Redis` is all we need here