import { AuthService } from '../interceptors/auth.service';
import { Injectable } from '@angular/core';
import { PermissionModel } from '../models/permission.model';
import { Permission } from '../models/permissions.enum';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  constructor(private auth: AuthService, private router: Router) {}

  permission: PermissionModel[] = [
    //#region Users
    {
      url: new RegExp('/home/users'),
      permissions: [
        Permission.ViewUser,
        Permission.CreateUser,
        Permission.DeleteUser,
        Permission.EditUser,
      ],
    },
    //#endregion

    //#region Brand
    {
      url: new RegExp('/home/brands'),
      permissions: [
        Permission.EditBrand,
        Permission.ViewBrand,
        Permission.CreateBrand,
      ],
    },
    //#endregion

    //#region Category
    {
      url: new RegExp('home/categories'),
      permissions: [
        Permission.ViewCategory,
        Permission.CreateCategory,
        Permission.EditCategory,
      ],
    },
    //#endregion

    //#region Customer
    {
      url: new RegExp('/home/customers'),
      permissions: [
        Permission.ViewCustomer,
        Permission.CreateCustomer,
        Permission.EditCustomer,
      ],
    },
    //#endregion

    //#region Invoice
    {
      url: new RegExp('/home/invoices'),
      permissions: [Permission.GetInvoice, Permission.CreateInvoice],
    },
    {
      url: new RegExp('/home/invoices/new'),
      permissions: [Permission.CreateInvoice],
    },
    {
      url: new RegExp('/home/invoices/edit/:id'),
      permissions: [Permission.CreateInvoice],
    },
    //#endregion
    
    //#region Receipt
    {
      url: new RegExp('/home/receipts'),
      permissions: [Permission.GetInvoice, Permission.CreateInvoice],
    },
    {
      url: new RegExp('/home/receipts/new'),
      permissions: [Permission.CreateInvoice],
    },
    {
      url: new RegExp('/home/receipts/edit/:id'),
      permissions: [Permission.CreateInvoice],
    },
    //#endregion
  

    //#region Product
    {
      url: new RegExp('/home/products'),
      permissions: [
        Permission.ViewProduct,
        Permission.CreateProduct,
        Permission.EditProduct,
      ],
    },
    //#endregion
    //#region Suppliers
    {
      url: new RegExp('/home/suppliers'),
      permissions: [
        Permission.ViewSupplier,
        Permission.EditSupplier,
        Permission.CreateSupplier,
      ],
    },
    //#endregion
    //#region OrderSupplier
    {
      url: new RegExp('/home/orders'),
      permissions: [
        Permission.ViewOrderSupplier,
        Permission.CreateOrderSupplier,
        Permission.EditOrderSupplier,
      ],
    },
    //#endregion
    //#region UpdatePrice
    {
      url: new RegExp('/home/products/updateprice'),
      permissions: [Permission.ListUpdatePrice, Permission.EditUpdatePrice],
    },
    //#endregion
    //#region UpdatePrice
    {
      url: new RegExp('/home/periods'),
      permissions: [
        Permission.CreatePeriod,
        Permission.GetPeriod,
        Permission.DeletePeriod,
        Permission.EditPeriod,
      ],
    },
    //#endregion  

    //#region Notes
    {
      url: new RegExp('/home/credits'),
      permissions: [
        Permission.GetMemo,
      ],
    },
    {
      url: new RegExp('/home/debits'),
      permissions: [
        Permission.GetMemo,
      ],
    }, 
    {
      url: new RegExp('/home/debits/edit/:id'),
      permissions: [
        Permission.CreateMemo,
      ],
    }, 
    {
      url: new RegExp('/home/credits/edit/:id'),
      permissions: [
        Permission.CreateMemo,
      ],
    }, 
    {
      url: new RegExp('/home/credits/new'),
      permissions: [
        Permission.CreateMemo,
      ],
    }, 
    {
      url: new RegExp('/home/debits/new'),
      permissions: [
        Permission.CreateMemo,
      ],
    }, 
    
    //#endregion
    //permission rol
    {
      url: new RegExp('/home/permission'),
      permissions: [
        Permission.RolControl,
      ],
    },

    //#regionIva
    {
      url: new RegExp('/home/iva/ivaCompra'),
      permissions: [
        Permission.ListIva,
      ],
    }, {
      url: new RegExp('/home/iva/ivaVenta'),
      permissions: [
        Permission.ListIva,
      ],
    },
    //#endrregion

      //#regionReporteZ
      {
        url: new RegExp('/home/report'),
        permissions: [
          Permission.ReportZ,
        ],
      },
      {
        url: new RegExp('/home/print/settings'),
        permissions: [
          Permission.ReportZ,
        ],
      },
      //#endrregion

      //#regionBudget
      {
        url: new RegExp('/home/budgets'),
        permissions: [Permission.GetBudget, Permission.CreateBudget],
      },
      //#endrregion

      //#regionDeliveryNote
      {
        url: new RegExp('/home/delivery-notes'),
        permissions: [Permission.GetRemito, Permission.CreateRemito],
      },
      {
        url: new RegExp('/home/delivery-notes/edit/:id'),
        permissions: [Permission.GetRemito, Permission.CreateRemito],
      },
      {
        url: new RegExp('/home/delivery-notes/new'),
        permissions: [Permission.GetRemito, Permission.CreateRemito],
      },
      //#endrregion
      //#regionQuittance
      {
        url: new RegExp('/home/quittance'),
        permissions: [Permission.GetQuittance, Permission.CreateQuittance],
      },
      //#endrregion
      {
        url: new RegExp('/home/products/report'),
        permissions: [Permission.ViewProduct,
          Permission.CreateProduct,
          Permission.EditProduct,],
      },
      //#region Company
      {
        url: new RegExp('/home/companies'),
        permissions: [Permission.CreateCompany, Permission.ViewCompany],
      },
      {
        url: new RegExp('/home/companies/new'),
        permissions: [Permission.CreateCompany, Permission.ViewCompany],
      },
      {
        url: new RegExp('/home/companies/edit/:id'),
        permissions: [Permission.CreateCompany, Permission.ViewCompany],
      },
      //#endrregion
  ];

  public hasPermission(url: string) {
    if (!this.auth.currentUser) {
      return this.router.navigate(['auth/login']);
    }

    let permissions = this.auth.currentUser.permission;

    let hasPermission = this.checkPermissions(url, permissions);
    if (!hasPermission) {
      this.router.navigate(['home']);
    }

    return hasPermission;
  }

  private checkPermissions(url: string, permissions: Permission[]): boolean {
    let permissionUrl = this.permission.find((p) =>
      p.url.test(url)
    )?.permissions;
    if (!permissionUrl) return false;

    return permissions.findIndex((i) => permissionUrl?.includes(i)) !== -1;
  }

  public validatePermissionKey(permissionKey: Permission[]): boolean {
    let userPerms = this.auth.currentUser.permission;

    let valid =
      userPerms && permissionKey.findIndex((i) => userPerms.includes(i)) !== -1;

    return valid;
  }
}
