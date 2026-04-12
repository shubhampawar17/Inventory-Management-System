import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CatalogWorkspaceComponent } from './components/catalog-workspace/catalog-workspace.component';
import { MovementsWorkspaceComponent } from './components/movements-workspace/movements-workspace.component';
import { OverviewWorkspaceComponent } from './components/overview-workspace/overview-workspace.component';
import { SuppliersWorkspaceComponent } from './components/suppliers-workspace/suppliers-workspace.component';
import { WorkspaceHeaderComponent } from './components/workspace-header/workspace-header.component';

@NgModule({
  declarations: [
    AppComponent,
    WorkspaceHeaderComponent,
    OverviewWorkspaceComponent,
    CatalogWorkspaceComponent,
    SuppliersWorkspaceComponent,
    MovementsWorkspaceComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
