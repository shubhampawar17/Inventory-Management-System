import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../environments/environment';
import { DashboardSummary, Inventory, InventoryTransaction, LoginResponse, Product, Supplier } from './models';

@Injectable({
  providedIn: 'root'
})
export class InventoryApiService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  getInventories(): Observable<Inventory[]> {
    return this.http.get<Inventory[]>(`${this.baseUrl}/inventories`);
  }

  login(payload: { username: string; password: string }): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/auth/login`, payload);
  }

  getDashboardSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${this.baseUrl}/dashboard/summary`);
  }

  createInventory(payload: { location: string }): Observable<Inventory> {
    return this.http.post<Inventory>(`${this.baseUrl}/inventories`, payload);
  }

  updateInventory(inventoryId: number, payload: { location: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/inventories/${inventoryId}`, payload);
  }

  deleteInventory(inventoryId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/inventories/${inventoryId}`);
  }

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}/products`);
  }

  createProduct(payload: Omit<Product, 'productId'>): Observable<Product> {
    return this.http.post<Product>(`${this.baseUrl}/products`, payload);
  }

  updateProduct(productId: number, payload: Omit<Product, 'productId'>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/products/${productId}`, payload);
  }

  deleteProduct(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/products/${productId}`);
  }

  getSuppliers(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>(`${this.baseUrl}/suppliers`);
  }

  createSupplier(payload: Omit<Supplier, 'supplierId'>): Observable<Supplier> {
    return this.http.post<Supplier>(`${this.baseUrl}/suppliers`, payload);
  }

  updateSupplier(supplierId: number, payload: Omit<Supplier, 'supplierId'>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/suppliers/${supplierId}`, payload);
  }

  deleteSupplier(supplierId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/suppliers/${supplierId}`);
  }

  getTransactions(): Observable<InventoryTransaction[]> {
    return this.http.get<InventoryTransaction[]>(`${this.baseUrl}/transactions`);
  }

  createTransaction(payload: { productId: number; type: string; quantity: number }): Observable<InventoryTransaction> {
    return this.http.post<InventoryTransaction>(`${this.baseUrl}/transactions`, payload);
  }
}
