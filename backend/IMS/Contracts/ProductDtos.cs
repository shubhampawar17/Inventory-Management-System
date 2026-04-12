namespace IMS.Contracts;

public record ProductRequest(
    string Name,
    string Description,
    int Quantity,
    double Price,
    int InventoryId);

public record ProductResponse(
    int ProductId,
    string Name,
    string Description,
    int Quantity,
    double Price,
    int InventoryId);
