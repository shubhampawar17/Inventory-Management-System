namespace IMS.Contracts;

public record InventoryRequest(
    string Location);

public record InventoryResponse(
    int InventoryId,
    string Location);
