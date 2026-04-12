import { Component, EventEmitter, Input, Output } from '@angular/core';

import { DashboardSummary, Inventory } from '../../models';

@Component({
  selector: 'app-workspace-header',
  templateUrl: './workspace-header.component.html',
  styleUrls: ['./workspace-header.component.css']
})
export class WorkspaceHeaderComponent {
  @Input() showcaseTitle = '';
  @Input() brandTitle = 'IMS';
  @Input() operationalHeadline = '';
  @Input() operationalSubline = '';
  @Input() lowStockThreshold = 10;
  @Input() summary!: DashboardSummary;
  @Input() inventories: Inventory[] = [];
  @Input() workspaceViews: Array<{ key: string; label: string; description: string }> = [];
  @Input() activeView = 'overview';
  @Input() inventoryFilterId = 'all';

  @Output() activeViewChange = new EventEmitter<string>();
  @Output() inventoryFilterIdChange = new EventEmitter<string>();
  @Output() refresh = new EventEmitter<void>();
  @Output() logout = new EventEmitter<void>();
}
