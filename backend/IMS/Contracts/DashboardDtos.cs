namespace IMS.Contracts;

public record DashboardSummaryResponse(
    DashboardKpiResponse Kpis,
    IReadOnlyList<InventorySnapshotResponse> InventorySnapshots,
    IReadOnlyList<LowStockProductResponse> LowStockProducts,
    IReadOnlyList<TopProductResponse> TopProducts,
    IReadOnlyList<RecentTransactionInsightResponse> RecentTransactions);

public record DashboardKpiResponse(
    int TotalInventories,
    int TotalProducts,
    int TotalSuppliers,
    int TotalTransactions,
    int TotalUnits,
    double TotalInventoryValue,
    int InStockRate,
    int SupplierCoverage,
    int AddTransactionCount,
    int RemoveTransactionCount,
    int LowStockCount);

public record InventorySnapshotResponse(
    int InventoryId,
    string Location,
    int ProductCount,
    int SupplierCount,
    int TotalUnits,
    double StockValue,
    string HealthLabel);

public record LowStockProductResponse(
    int ProductId,
    string ProductName,
    string InventoryLocation,
    int InventoryId,
    int Quantity);

public record TopProductResponse(
    int ProductId,
    string ProductName,
    string InventoryLocation,
    int InventoryId,
    int Quantity,
    double Price,
    double ExposureValue);

public record RecentTransactionInsightResponse(
    int TransactionId,
    int ProductId,
    string ProductName,
    string InventoryLocation,
    int InventoryId,
    string Type,
    int Quantity,
    DateTime Date);
