import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { forkJoin } from 'rxjs';

import { InventoryApiService } from './inventory-api.service';
import {
  DashboardSummary,
  Inventory,
  InventoryTransaction,
  LowStockProduct,
  Product,
  ProductWorkspaceRow,
  Supplier,
  SupplierWorkspaceCard,
  TransactionWorkspaceRow
} from './models';

type WorkspaceView = 'overview' | 'catalog' | 'suppliers' | 'movements';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit {
  readonly showcaseTitle = 'Northstar Inventory Console';
  readonly lowStockThreshold = 10;
  readonly workspaceViews: { key: WorkspaceView; label: string; description: string }[] = [
    { key: 'overview', label: 'Overview', description: 'Executive metrics and warehouse health' },
    { key: 'catalog', label: 'Catalog', description: 'Product mix, stock exposure, and edits' },
    { key: 'suppliers', label: 'Suppliers', description: 'Vendor footprint and contact readiness' },
    { key: 'movements', label: 'Movements', description: 'Inbound and outbound stock activity' }
  ];

  dashboardSummary: DashboardSummary = createEmptyDashboardSummary();
  inventories: Inventory[] = [];
  products: Product[] = [];
  suppliers: Supplier[] = [];
  transactions: InventoryTransaction[] = [];
  message = '';
  messageType: 'success' | 'error' = 'success';
  isLoading = true;
  isAuthenticated = false;
  activeView: WorkspaceView = 'overview';
  loginError = '';
  generatedCaptcha = '';

  loginForm = {
    username: '',
    password: '',
    captchaInput: ''
  };

  inventoryFilterId = 'all';
  productSearchTerm = '';
  supplierSearchTerm = '';
  transactionTypeFilter = 'all';
  transactionSearchTerm = '';
  highlightedProductId: number | null = null;

  inventoryForm = {
    location: ''
  };
  editingInventoryId: number | null = null;

  productForm: {
    productId: number | null;
    name: string;
    description: string;
    quantity: number;
    price: number;
    inventoryId: number;
  } = this.createEmptyProductForm();

  supplierForm: {
    supplierId: number | null;
    name: string;
    contactInformation: string;
    inventoryId: number;
  } = this.createEmptySupplierForm();

  transactionForm = {
    productId: 0,
    type: 'Add',
    quantity: 1
  };

  constructor(private readonly api: InventoryApiService) {}

  ngOnInit(): void {
    sessionStorage.removeItem('ims-authenticated');
    this.isAuthenticated = false;
    this.refreshCaptcha();
    this.isLoading = false;
  }

  get operationalHeadline(): string {
    if (!this.dashboardSummary.kpis.totalProducts) {
      return 'Seed the first inventory to unlock analytics and stock insights.';
    }

    if (this.dashboardSummary.kpis.lowStockCount > 0) {
      return `${this.dashboardSummary.kpis.lowStockCount} SKUs need replenishment attention across your network.`;
    }

    return 'All tracked SKUs are above your low-stock threshold.';
  }

  get operationalSubline(): string {
    if (!this.dashboardSummary.kpis.totalTransactions) {
      return 'No stock movements recorded yet. Start posting transactions to show live operational history.';
    }

    return `${this.dashboardSummary.kpis.addTransactionCount} inbound and ${this.dashboardSummary.kpis.removeTransactionCount} outbound movements are currently captured.`;
  }

  get filteredProductRows(): ProductWorkspaceRow[] {
    const searchTerm = this.productSearchTerm.trim().toLowerCase();

    return this.products
      .filter(product => {
        const matchesInventory = this.inventoryFilterId === 'all' || product.inventoryId === Number(this.inventoryFilterId);
        const matchesSearch = !searchTerm
          || product.name.toLowerCase().includes(searchTerm)
          || product.description.toLowerCase().includes(searchTerm);

        return matchesInventory && matchesSearch;
      })
      .map(product => ({
        productId: product.productId,
        name: product.name,
        description: product.description,
        status: this.getProductStatus(product),
        inventoryId: product.inventoryId,
        inventoryLocation: this.getInventoryLocation(product.inventoryId),
        quantity: product.quantity,
        price: product.price,
        exposureValue: product.quantity * product.price
      }));
  }

  get filteredSupplierCards(): SupplierWorkspaceCard[] {
    const searchTerm = this.supplierSearchTerm.trim().toLowerCase();

    return this.suppliers
      .filter(supplier => {
        const matchesInventory = this.inventoryFilterId === 'all' || supplier.inventoryId === Number(this.inventoryFilterId);
        const matchesSearch = !searchTerm
          || supplier.name.toLowerCase().includes(searchTerm)
          || supplier.contactInformation.toLowerCase().includes(searchTerm);

        return matchesInventory && matchesSearch;
      })
      .map(supplier => ({
        supplierId: supplier.supplierId,
        name: supplier.name,
        contactInformation: supplier.contactInformation,
        inventoryId: supplier.inventoryId,
        inventoryLocation: this.getInventoryLocation(supplier.inventoryId)
      }));
  }

  get filteredTransactionRows(): TransactionWorkspaceRow[] {
    const searchTerm = this.transactionSearchTerm.trim().toLowerCase();

    return this.transactions
      .map(transaction => ({
        transactionId: transaction.transactionId,
        productId: transaction.productId,
        productName: this.getProductName(transaction.productId),
        type: transaction.type,
        normalizedType: normalizeTransactionType(transaction.type),
        quantity: transaction.quantity,
        inventoryId: transaction.inventoryId,
        inventoryLocation: this.getInventoryLocation(transaction.inventoryId),
        date: transaction.date
      }))
      .filter(transaction => {
        const matchesInventory = this.inventoryFilterId === 'all' || transaction.inventoryId === Number(this.inventoryFilterId);
        const matchesType = this.transactionTypeFilter === 'all' || transaction.normalizedType === this.transactionTypeFilter;
        const matchesSearch = !searchTerm
          || transaction.productName.toLowerCase().includes(searchTerm)
          || transaction.normalizedType.toLowerCase().includes(searchTerm);

        return matchesInventory && matchesType && matchesSearch;
      });
  }

  get hasInventoryLocation(): boolean {
    return this.inventoryForm.location.trim().length > 0;
  }

  login(): void {
    const username = this.loginForm.username.trim();
    const password = this.loginForm.password;
    const captchaInput = this.loginForm.captchaInput.trim();

    if (!username || !password || !captchaInput) {
      this.loginError = 'Enter username, password, and captcha.';
      return;
    }

    if (captchaInput !== this.generatedCaptcha) {
      this.loginError = 'Captcha does not match.';
      this.loginForm.captchaInput = '';
      this.refreshCaptcha();
      return;
    }

    this.api.login({ username, password }).subscribe({
      next: response => {
        if (!response.isAuthenticated) {
          this.loginError = response.message;
          this.loginForm.password = '';
          this.loginForm.captchaInput = '';
          this.refreshCaptcha();
          return;
        }

        this.loginError = '';
        this.isAuthenticated = true;
        this.loadDashboard();
      },
      error: (error: HttpErrorResponse) => {
        this.loginError = error.error?.message ?? 'Login failed.';
        this.loginForm.password = '';
        this.loginForm.captchaInput = '';
        this.refreshCaptcha();
      }
    });
  }

  logout(): void {
    this.isAuthenticated = false;
    this.isLoading = false;
    this.message = '';
    this.loginError = '';
    this.loginForm = {
      username: '',
      password: '',
      captchaInput: ''
    };
    this.refreshCaptcha();
  }

  refreshCaptcha(): void {
    const alphabet = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
    this.generatedCaptcha = Array.from({ length: 6 }, () =>
      alphabet[Math.floor(Math.random() * alphabet.length)]
    ).join('');
  }

  loadDashboard(): void {
    this.isLoading = true;

    forkJoin({
      summary: this.api.getDashboardSummary(),
      inventories: this.api.getInventories(),
      products: this.api.getProducts(),
      suppliers: this.api.getSuppliers(),
      transactions: this.api.getTransactions()
    }).subscribe({
      next: ({ summary, inventories, products, suppliers, transactions }) => {
        this.dashboardSummary = summary;
        this.inventories = inventories;
        this.products = products;
        this.suppliers = suppliers;
        this.transactions = transactions.sort((left, right) => toDateValue(right.date) - toDateValue(left.date));
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.setMessage('Unable to load data. Start the API and check the database connection.', 'error');
      }
    });
  }

  setActiveView(view: string): void {
    this.activeView = view as WorkspaceView;
  }

  saveInventory(): void {
    const location = this.inventoryForm.location.trim();

    if (!location) {
      return;
    }

    const payload = { location };

    if (this.editingInventoryId !== null) {
      this.api.updateInventory(this.editingInventoryId, payload).subscribe({
        next: () => {
          this.resetInventoryForm();
          this.loadDashboard();
          this.setMessage('Inventory node renamed.');
        },
        error: (error: HttpErrorResponse) => this.setMessage(error.error?.message ?? 'Failed to save inventory.', 'error')
      });
      return;
    }

    this.api.createInventory(payload).subscribe({
      next: () => {
        this.resetInventoryForm();
        this.loadDashboard();
        this.setMessage('Inventory node created.');
      },
      error: (error: HttpErrorResponse) => this.setMessage(error.error?.message ?? 'Failed to save inventory.', 'error')
    });
  }

  editInventory(inventory: Inventory): void {
    this.editingInventoryId = inventory.inventoryId;
    this.inventoryForm = {
      location: inventory.location
    };
  }

  cancelInventoryEdit(): void {
    this.resetInventoryForm();
  }

  deleteInventory(inventory: Inventory): void {
    const confirmed = window.confirm(
      `Delete inventory '${inventory.location}'? This works only when it has no products, suppliers, or transactions.`
    );

    if (!confirmed) {
      return;
    }

    this.api.deleteInventory(inventory.inventoryId).subscribe({
      next: () => {
        if (this.inventoryFilterId === String(inventory.inventoryId)) {
          this.inventoryFilterId = 'all';
        }

        if (this.editingInventoryId === inventory.inventoryId) {
          this.resetInventoryForm();
        }

        this.loadDashboard();
        this.setMessage('Inventory node deleted.');
      },
      error: (error: HttpErrorResponse) => this.setMessage(error.error?.message ?? 'Failed to delete inventory.', 'error')
    });
  }

  saveProduct(): void {
    const payload = {
      name: this.productForm.name,
      description: this.productForm.description,
      quantity: Number(this.productForm.quantity),
      price: Number(this.productForm.price),
      inventoryId: Number(this.productForm.inventoryId)
    };

    if (this.productForm.productId) {
      this.api.updateProduct(this.productForm.productId, payload).subscribe({
        next: () => {
          this.highlightedProductId = null;
          this.resetProductForm();
          this.loadDashboard();
          this.setMessage('Product updated.');
        },
        error: (error: HttpErrorResponse) => this.setMessage(error.error?.message ?? 'Failed to save product.', 'error')
      });
      return;
    }

    this.api.createProduct(payload).subscribe({
      next: createdProduct => {
        this.highlightedProductId = createdProduct.productId;
        this.resetProductForm();
        this.loadDashboard();
        this.setMessage('Product created.');
      },
      error: (error: HttpErrorResponse) => this.setMessage(error.error?.message ?? 'Failed to save product.', 'error')
    });
  }

  editProduct(product: Product): void {
    this.activeView = 'catalog';
    this.productForm = { ...product };
  }

  deleteProduct(productId: number): void {
    this.api.deleteProduct(productId).subscribe({
      next: () => {
        if (this.highlightedProductId === productId) {
          this.highlightedProductId = null;
        }

        this.loadDashboard();
        this.setMessage('Product deleted.');
      },
      error: (error: HttpErrorResponse) => this.setMessage(error.error?.message ?? 'Failed to delete product.', 'error')
    });
  }

  resetProductForm(): void {
    this.productForm = this.createEmptyProductForm();
  }

  saveSupplier(): void {
    const payload = {
      name: this.supplierForm.name,
      contactInformation: this.supplierForm.contactInformation,
      inventoryId: Number(this.supplierForm.inventoryId)
    };

    if (this.supplierForm.supplierId) {
      this.api.updateSupplier(this.supplierForm.supplierId, payload).subscribe({
        next: () => {
          this.resetSupplierForm();
          this.loadDashboard();
          this.setMessage('Supplier updated.');
        },
        error: (error: HttpErrorResponse) => this.setMessage(error.error?.message ?? 'Failed to save supplier.', 'error')
      });
      return;
    }

    this.api.createSupplier(payload).subscribe({
      next: () => {
        this.resetSupplierForm();
        this.loadDashboard();
        this.setMessage('Supplier created.');
      },
      error: (error: HttpErrorResponse) => this.setMessage(error.error?.message ?? 'Failed to save supplier.', 'error')
    });
  }

  editSupplier(supplier: Supplier): void {
    this.activeView = 'suppliers';
    this.supplierForm = { ...supplier };
  }

  deleteSupplier(supplierId: number): void {
    this.api.deleteSupplier(supplierId).subscribe({
      next: () => {
        this.loadDashboard();
        this.setMessage('Supplier deleted.');
      },
      error: () => this.setMessage('Failed to delete supplier.', 'error')
    });
  }

  resetSupplierForm(): void {
    this.supplierForm = this.createEmptySupplierForm();
  }

  createTransaction(): void {
    this.api.createTransaction({
      productId: Number(this.transactionForm.productId),
      type: this.transactionForm.type,
      quantity: Number(this.transactionForm.quantity)
    }).subscribe({
      next: () => {
        this.transactionForm = {
          productId: 0,
          type: 'Add',
          quantity: 1
        };
        this.loadDashboard();
        this.setMessage('Transaction saved.');
      },
      error: (error: HttpErrorResponse) => this.setMessage(error.error?.message ?? 'Failed to save transaction.', 'error')
    });
  }

  stageTransaction(productId: number, type: 'Add' | 'Remove' = 'Add'): void {
    this.activeView = 'movements';
    this.transactionForm = {
      productId,
      type,
      quantity: this.transactionForm.quantity
    };
  }

  stageRestock(product: LowStockProduct): void {
    this.stageTransaction(product.productId, 'Add');
  }

  getProductName(productId: number): string {
    return this.products.find(product => product.productId === productId)?.name ?? `Product #${productId}`;
  }

  getInventoryLocation(inventoryId: number): string {
    return this.inventories.find(inventory => inventory.inventoryId === inventoryId)?.location ?? `Inventory #${inventoryId}`;
  }

  private getProductStatus(product: Product): 'critical' | 'watch' | 'healthy' {
    if (product.quantity <= 5) {
      return 'critical';
    }

    if (product.quantity <= this.lowStockThreshold) {
      return 'watch';
    }

    return 'healthy';
  }

  private setMessage(message: string, type: 'success' | 'error' = 'success'): void {
    this.message = message;
    this.messageType = type;
  }

  closeMessageDialog(): void {
    this.message = '';
  }

  private resetInventoryForm(): void {
    this.editingInventoryId = null;
    this.inventoryForm = {
      location: ''
    };
  }

  private createEmptyProductForm() {
    return {
      productId: null,
      name: '',
      description: '',
      quantity: 0,
      price: 0,
      inventoryId: 0
    };
  }

  private createEmptySupplierForm() {
    return {
      supplierId: null,
      name: '',
      contactInformation: '',
      inventoryId: 0
    };
  }
}

function createEmptyDashboardSummary(): DashboardSummary {
  return {
    kpis: {
      totalInventories: 0,
      totalProducts: 0,
      totalSuppliers: 0,
      totalTransactions: 0,
      totalUnits: 0,
      totalInventoryValue: 0,
      inStockRate: 0,
      supplierCoverage: 0,
      addTransactionCount: 0,
      removeTransactionCount: 0,
      lowStockCount: 0
    },
    inventorySnapshots: [],
    lowStockProducts: [],
    topProducts: [],
    recentTransactions: []
  };
}

function toDateValue(value: string): number {
  return new Date(value).getTime();
}

function normalizeTransactionType(type: string): 'Add' | 'Remove' {
  return type.toLowerCase().includes('remove') ? 'Remove' : 'Add';
}

