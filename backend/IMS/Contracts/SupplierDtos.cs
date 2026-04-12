namespace IMS.Contracts;

public record SupplierRequest(
    string Name,
    string ContactInformation,
    int InventoryId);

public record SupplierResponse(
    int SupplierId,
    string Name,
    string ContactInformation,
    int InventoryId);
