export interface Inventory {
  inventoryId: number;
  location: string;
}

export interface LoginResponse {
  isAuthenticated: boolean;
  message: string;
  username: string;
}

export interface Product {
  productId: number;
  name: string;
  description: string;
  quantity: number;
  price: number;
  inventoryId: number;
}

export interface Supplier {
  supplierId: number;
  name: string;
  contactInformation: string;
  inventoryId: number;
}

export interface InventoryTransaction {
  transactionId: number;
  productId: number;
  type: string;
  quantity: number;
  date: string;
  inventoryId: number;
}

export interface DashboardSummary {
  kpis: DashboardKpis;
  inventorySnapshots: InventorySnapshot[];
  lowStockProducts: LowStockProduct[];
  topProducts: TopProduct[];
  recentTransactions: RecentTransactionInsight[];
}

export interface DashboardKpis {
  totalInventories: number;
  totalProducts: number;
  totalSuppliers: number;
  totalTransactions: number;
  totalUnits: number;
  totalInventoryValue: number;
  inStockRate: number;
  supplierCoverage: number;
  addTransactionCount: number;
  removeTransactionCount: number;
  lowStockCount: number;
}

export interface InventorySnapshot {
  inventoryId: number;
  location: string;
  productCount: number;
  supplierCount: number;
  totalUnits: number;
  stockValue: number;
  healthLabel: string;
}

export interface LowStockProduct {
  productId: number;
  productName: string;
  inventoryLocation: string;
  inventoryId: number;
  quantity: number;
}

export interface TopProduct {
  productId: number;
  productName: string;
  inventoryLocation: string;
  inventoryId: number;
  quantity: number;
  price: number;
  exposureValue: number;
}

export interface RecentTransactionInsight {
  transactionId: number;
  productId: number;
  productName: string;
  inventoryLocation: string;
  inventoryId: number;
  type: string;
  quantity: number;
  date: string;
}

export interface ProductWorkspaceRow {
  productId: number;
  name: string;
  description: string;
  status: 'critical' | 'watch' | 'healthy';
  inventoryId: number;
  inventoryLocation: string;
  quantity: number;
  price: number;
  exposureValue: number;
}

export interface SupplierWorkspaceCard {
  supplierId: number;
  name: string;
  contactInformation: string;
  inventoryId: number;
  inventoryLocation: string;
}

export interface TransactionWorkspaceRow {
  transactionId: number;
  productId: number;
  productName: string;
  type: string;
  normalizedType: 'Add' | 'Remove';
  quantity: number;
  inventoryId: number;
  inventoryLocation: string;
  date: string;
}
