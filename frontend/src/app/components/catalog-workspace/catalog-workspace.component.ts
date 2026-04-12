import { Component, EventEmitter, Input, Output } from '@angular/core';

import { Inventory, Product, ProductWorkspaceRow } from '../../models';

@Component({
  selector: 'app-catalog-workspace',
  templateUrl: './catalog-workspace.component.html',
  styleUrls: ['./catalog-workspace.component.css']
})
export class CatalogWorkspaceComponent {
  @Input() productForm!: {
    productId: number | null;
    name: string;
    description: string;
    quantity: number;
    price: number;
    inventoryId: number;
  };
  @Input() inventories: Inventory[] = [];
  @Input() productSearchTerm = '';
  @Input() highlightedProductId: number | null = null;
  @Input() rows: ProductWorkspaceRow[] = [];

  @Output() productSearchTermChange = new EventEmitter<string>();
  @Output() save = new EventEmitter<void>();
  @Output() reset = new EventEmitter<void>();
  @Output() editProduct = new EventEmitter<Product>();
  @Output() deleteProduct = new EventEmitter<number>();
  @Output() moveProduct = new EventEmitter<number>();

  onEdit(row: ProductWorkspaceRow): void {
    this.editProduct.emit({
      productId: row.productId,
      name: row.name,
      description: row.description,
      quantity: row.quantity,
      price: row.price,
      inventoryId: row.inventoryId
    });
  }

  onMove(row: ProductWorkspaceRow): void {
    this.moveProduct.emit(row.productId);
  }

  onDelete(row: ProductWorkspaceRow): void {
    this.deleteProduct.emit(row.productId);
  }
}
