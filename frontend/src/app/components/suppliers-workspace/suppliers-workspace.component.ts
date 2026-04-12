import { Component, EventEmitter, Input, Output } from '@angular/core';

import { Inventory, Supplier, SupplierWorkspaceCard } from '../../models';

@Component({
  selector: 'app-suppliers-workspace',
  templateUrl: './suppliers-workspace.component.html',
  styleUrls: ['./suppliers-workspace.component.css']
})
export class SuppliersWorkspaceComponent {
  @Input() supplierForm!: {
    supplierId: number | null;
    name: string;
    contactInformation: string;
    inventoryId: number;
  };
  @Input() inventories: Inventory[] = [];
  @Input() supplierCoverage = 0;
  @Input() supplierSearchTerm = '';
  @Input() cards: SupplierWorkspaceCard[] = [];

  @Output() supplierSearchTermChange = new EventEmitter<string>();
  @Output() save = new EventEmitter<void>();
  @Output() reset = new EventEmitter<void>();
  @Output() edit = new EventEmitter<Supplier>();
  @Output() delete = new EventEmitter<number>();
}
