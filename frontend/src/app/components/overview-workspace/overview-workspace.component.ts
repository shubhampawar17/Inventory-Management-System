import { Component, EventEmitter, Input, Output } from '@angular/core';

import { DashboardSummary, LowStockProduct } from '../../models';

@Component({
  selector: 'app-overview-workspace',
  templateUrl: './overview-workspace.component.html',
  styleUrls: ['./overview-workspace.component.css']
})
export class OverviewWorkspaceComponent {
  @Input() summary!: DashboardSummary;
  @Output() stageTransaction = new EventEmitter<LowStockProduct>();
  @Output() openMovements = new EventEmitter<void>();
}
