namespace IMS.Contracts;

public record TransactionRequest(
    int ProductId,
    string Type,
    int Quantity);

public record TransactionResponse(
    int TransactionId,
    int ProductId,
    string Type,
    int Quantity,
    DateTime Date,
    int InventoryId);
