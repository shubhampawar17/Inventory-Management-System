import { Component, EventEmitter, Input, Output } from '@angular/core';

import { Product, TransactionWorkspaceRow } from '../../models';

@Component({
  selector: 'app-movements-workspace',
  templateUrl: './movements-workspace.component.html',
  styleUrls: ['./movements-workspace.component.css']
})
export class MovementsWorkspaceComponent {
  @Input() transactionForm!: {
    productId: number;
    type: string;
    quantity: number;
  };
  @Input() products: Product[] = [];
  @Input() transactionSearchTerm = '';
  @Input() transactionTypeFilter = 'all';
  @Input() rows: TransactionWorkspaceRow[] = [];

  @Output() transactionSearchTermChange = new EventEmitter<string>();
  @Output() transactionTypeFilterChange = new EventEmitter<string>();
  @Output() save = new EventEmitter<void>();
}
