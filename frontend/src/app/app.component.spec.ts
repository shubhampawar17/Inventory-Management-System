import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';

import { AppComponent } from './app.component';
import { CatalogWorkspaceComponent } from './components/catalog-workspace/catalog-workspace.component';
import { MovementsWorkspaceComponent } from './components/movements-workspace/movements-workspace.component';
import { OverviewWorkspaceComponent } from './components/overview-workspace/overview-workspace.component';
import { SuppliersWorkspaceComponent } from './components/suppliers-workspace/suppliers-workspace.component';
import { WorkspaceHeaderComponent } from './components/workspace-header/workspace-header.component';
import { InventoryApiService } from './inventory-api.service';

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormsModule, HttpClientTestingModule],
      declarations: [
        AppComponent,
        WorkspaceHeaderComponent,
        OverviewWorkspaceComponent,
        CatalogWorkspaceComponent,
        SuppliersWorkspaceComponent,
        MovementsWorkspaceComponent
      ],
      providers: [
        {
          provide: InventoryApiService,
          useValue: {
            getDashboardSummary: () => of({
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
            }),
            getInventories: () => of([]),
            getProducts: () => of([]),
            getSuppliers: () => of([]),
            getTransactions: () => of([]),
            createInventory: () => of({}),
            createProduct: () => of({}),
            updateProduct: () => of(void 0),
            deleteProduct: () => of(void 0),
            createSupplier: () => of({}),
            updateSupplier: () => of(void 0),
            deleteSupplier: () => of(void 0),
            createTransaction: () => of({})
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
  });

  it('creates the app', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('renders the showcase heading', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Northstar Inventory Console');
  });
});
