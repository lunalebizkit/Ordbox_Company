
import { ApiService } from '../../../common/services/api.base.service';
import { Observable } from 'rxjs';
import { CustomerAddModel } from './model/customer.add.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class EntityService {
  /**
   * Constructor
   */
  constructor(public api: ApiService) {}
  /**
   * Obtiene todas los Proveedores
   * @param queryParams
   * @returns
   */
  public getSuppliers(queryParams: any): Observable<any> {
    return this.api.post(`Supplier/list`, queryParams, false);
  }
  /**
   * Obtiene un proveedor por Id
   * @param id
   * @returns
   */
  public getSupplierById(id: number): Observable<any> {
    return this.api.get(`supplier?id=${id}`, false);
  }

  /**
   * Obtiene un Proveedor por Cuit
   * @param cuit
   * @returns
   */
  public getSupplierByCuit(cuit: string | number): Observable<any> {
    return this.api.get(`supplier/GetSupplierByCuit?cuit=${cuit}`, false);
  }

  /**
   * Guarda un Proveedor
   * @param model
   * @returns
   */
  public saveSupplier(model: CustomerAddModel): Observable<any> {
    if (model.id === 0) {
      return this.api.post(`supplier`, model, false);
    } else {
      return this.api.put(`supplier`, model, false);
    }
  }
  /**
   * Obtiene todos los Clientes
   * @param queryParams
   * @returns
   */
  public getCustomers(queryParams: any): Observable<any> {
    return this.api.post(`Customer/list`, queryParams, false);
  }

  /**
   * Obtiene un Cliente por Id
   * @param id
   * @returns
   */
  public getById(id: string | number): Observable<any> {
    return this.api.get(`Customer?id=${id}`, false);
  }

  /**
   * Obtiene un Cliente por Cuit
   * @param cuit
   * @returns
   */
  public getByCuit(cuit: string | number): Observable<any> {
    return this.api.get(`Customer/GetCustomerByCuit?cuit=${cuit}`, false);
  }

  /**
   * Guarda un Cliente
   * @param model
   * @returns
   */
  public saveCustomer(model: CustomerAddModel): Observable<any> {
    if (model.id === 0) {
      return this.api.post(`Customer`, model, false);
    } else {
      return this.api.put(`Customer`, model, false);
    }
  }
  /**
     * Elimina un cliente por Id
     * @param id
     * @returns
     */
  public deleteCustomer(id: number): Observable<any> {
    return this.api.delete(`entity/${id}`, false);
  }

  /**
   * Obtiene un Proveedor por Cuit
   * @param cuit
   * @returns
   */
  public getCustomersByCuit(cuit: string): Observable<any> {
    return this.api.get(`Entity/getcustomersbycuit?cuit=${cuit}`, false);
  }
}
