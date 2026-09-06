import { Routes } from '@angular/router';
import { AuthGuard } from './common/auth/permission/auth.guard';

export const routes: Routes = [
    { path: '', redirectTo: 'auth', pathMatch: 'full' },
     {
    path: 'auth',
    loadComponent: () =>
      import('./pages/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'home',
    loadComponent: () =>
      import('./pages/home/home.component').then(m => m.HomeComponent),
      children: [
      {
        canActivate: [AuthGuard],
        path: 'users',
        loadComponent: () =>
          import('./pages/home/users/users-list/users-list.component').then(m => m.UsersListComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'users/new',
        loadComponent: () =>
          import('./pages/home/users/users-edit/users-edit.component').then(m => m.UsersEditComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'users/edit/:id',
        loadComponent: () =>
          import('./pages/home/users/users-edit/users-edit.component').then(m => m.UsersEditComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'suppliers',
        loadComponent: () =>
          import('./pages/home/suppliers/suppliers-list/suppliers-list.component').then(m => m.SuppliersListComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'suppliers/new',
        loadComponent: () =>
          import('./pages/home/suppliers/suppliers-edit/suppliers-edit.component').then(m => m.SuppliersEditComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'suppliers/edit/:id',
        loadComponent: () =>
          import('./pages/home/suppliers/suppliers-edit/suppliers-edit.component').then(m => m.SuppliersEditComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'customers',
        loadComponent: () =>
          import('./pages/home/customers/customers-list/customers-list.component').then(m => m.CustomersListComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'customers/edit/:id',
        loadComponent: () =>
          import('./pages/home/customers/customers-edit/customers-edit.component').then(m => m.CustomersEditComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'customers/new',
        loadComponent: () =>
          import('./pages/home/customers/customers-edit/customers-edit.component').then(m => m.CustomersEditComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'permission',
        loadComponent: () =>
          import('./pages/home/permission-rol/permission/permission-rol.component').then((m) => m.PermissionRolComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'brands',
        loadComponent: () =>
          import('./pages/home/brands/brands-list/brands-list.component').then((m) => m.BrandsListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'brands/new',
        loadComponent: () =>
          import('./pages/home/brands/brands-edit/brands-edit.component').then((m) => m.BrandsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'brands/edit/:id',
        loadComponent: () =>
          import('./pages/home/brands/brands-edit/brands-edit.component').then((m) => m.BrandsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'products',
        loadComponent: () =>
          import('./pages/home/products/products-list/products-list.component').then((m) => m.ProductsListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'products/new',
        loadComponent: () =>
          import('./pages/home/products/products-edit/products-edit.component').then((m) => m.ProductsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'products/edit/:id',
        loadComponent: () =>
          import('./pages/home/products/products-edit/products-edit.component').then((m) => m.ProductsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'categories',
        loadComponent: () =>
          import('./pages/home/categories/categories-list/categories-list.component').then((m) => m.CategoriesListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'categories/new',
        loadComponent: () =>
          import('./pages/home/categories/categories-edit/categories-edit.component').then((m) => m.CategoryEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'categories/edit/:id',
        loadComponent: () =>
          import('./pages/home/categories/categories-edit/categories-edit.component').then((m) => m.CategoryEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'invoices',
        loadComponent: () =>
          import('./pages/home/invoices/invoices-list/invoices-list.component').then((m) => m.InvoicesListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'invoices/new',
        loadComponent: () =>
          import('./pages/home/invoices/invoices-edit/invoices-edit.component').then((m)=> m.InvoicesEditComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'invoices/edit/:id',
        loadComponent: () =>
          import('./pages/home/invoices/invoices-edit/invoices-edit.component').then((m)=> m.InvoicesEditComponent)
      },
      {
        canActivate: [AuthGuard],
        path: 'orders',
        loadComponent: () =>
          import('./pages/home/orders/orders-list/orders-list.component').then((m) => m.OrdersListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'orders/new',
        loadComponent: () =>
          import('./pages/home/orders/orders-edit/orders-edit.component').then((m) => m.OrdersEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'orders/edit/:id',
        loadComponent: () =>
          import('./pages/home/orders/orders-edit/orders-edit.component').then((m) => m.OrdersEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'receipts',
        loadComponent: () =>
          import('./pages/home/invoices/receipt-list/receipt-list.component').then((m) => m.ReceiptListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'receipts/new',
        loadComponent: () =>
          import('./pages/home/invoices/receipt-edit/receipt-edit.component').then((m) => m.ReceiptEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'receipts/edit/:id',
        loadComponent: () =>
          import('./pages/home/invoices/receipt-edit/receipt-edit.component').then((m) => m.ReceiptEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'credits',
        loadComponent: () =>
          import('./pages/home/notes/credits-list/credits-list.component').then((m) => m.CreditsListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'credits/new',
        loadComponent: () =>
          import('./pages/home/notes/credits-edit/credits-edit.component').then((m) => m.CreditsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'credits/edit/:id',
        loadComponent: () =>
          import('./pages/home/notes/credits-edit/credits-edit.component').then((m) => m.CreditsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'debits',
        loadComponent: () =>
          import('./pages/home/notes/debits-list/debits-list.component').then((m) => m.DebitMemoListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'debits/new',
        loadComponent: () =>
          import('./pages/home/notes/debits-edit/debits-edit.component').then((m) => m.DebitsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'debits/edit/:id',
        loadComponent: () =>
          import('./pages/home/notes/debits-edit/debits-edit.component').then((m) => m.DebitsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'budgets',
        loadComponent: () =>
          import('./pages/home/budgets/budgets-list/budgets-list.component').then((m) => m.BudgetsListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'budgets/new',
        loadComponent: () =>
          import('./pages/home/budgets/budgets-edit/budgets-edit.component').then((m) => m.BudgetsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'budgets/edit/:id',
        loadComponent: () =>
          import('./pages/home/budgets/budgets-edit/budgets-edit.component').then((m) => m.BudgetsEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'delivery-notes',
        loadComponent: () =>
          import('./pages/home/delivery-notes/delivery-notes-list/delivery-notes-list.component').then((m) => m.DeliveryNotesListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'delivery-notes/new',
        loadComponent: () =>
          import('./pages/home/delivery-notes/delivery-notes-edit/delivery-notes-edit.component').then((m) => m.DeliveryNotesEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'delivery-notes/edit/:id',
        loadComponent: () =>
          import('./pages/home/delivery-notes/delivery-notes-edit/delivery-notes-edit.component').then((m) => m.DeliveryNotesEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'quittances',
        loadComponent: () =>
          import('./pages/home/quittance/quittance-list/quittance-list.component').then((m) => m.QuittanceListComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'quittances/new',
        loadComponent: () =>
          import('./pages/home/quittance/quittance-edit/quittance-edit.component').then((m) => m.QuittanceEditComponent),
      },
      {
        canActivate: [AuthGuard],
        path: 'quittances/edit/:id',
        loadComponent: () =>
          import('./pages/home/quittance/quittance-edit/quittance-edit.component').then((m) => m.QuittanceEditComponent),
      },
    ]
  },
];
