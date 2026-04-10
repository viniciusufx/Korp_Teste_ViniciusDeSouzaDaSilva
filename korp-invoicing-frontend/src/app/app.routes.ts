import { Routes } from '@angular/router';

import { ProductsComponent } from './pages/products/products';
import { InvoicesComponent } from './pages/invoices/invoices';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'products',
    pathMatch: 'full'
  },
  {
    path: 'products',
    component: ProductsComponent
  },
  {
    path: 'invoices',
    component: InvoicesComponent
  },
  {
    path: '**',
    redirectTo: 'products'
  }
];