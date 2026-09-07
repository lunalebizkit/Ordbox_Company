
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ApiService } from '../../../common/services/api.base.service';
import { CompanyModel } from './model/company.model';

@Injectable({
    providedIn: 'root',
  })
  export class CompanyService {
    constructor(private api: ApiService) {}
  
    /**
     * Obtiene los compañia por Id
     * @param id
     * @returns
     */
    public getById(id: number): Observable<any> {
      return this.api.get(`company?id=${id}`, false);
    }
  
    /**
     * Obtiene los compañia por filtro
     * @param data
     * @returns
     */
    public getByFilter(data: any): Observable<any> {
      return this.api.post(`company/list`, data, false);
    }
    
  
    /**
     * Guarda un usuario
     * @param id
     * @returns
     */
    public saveCompany(model: CompanyModel): Observable<any> {
      if (model.id === 0) {
        return this.api.post(`company`, model, false);
      } else {
        return this.api.put(`company`, model, false);
      }
    }
  
    /**
     * Elimina un usuario por Id
     * @param id
     * @returns
     */
     public deleteCompany(id: number): Observable<any> {
      return this.api.delete(`company/${id}`, false);
    }
  
        /**
     * Obtiene los compañia por filtro
     * @param data
     * @returns
     */
      public getPermissions(): Observable<any> {
        return this.api.post(`rol/listpermissions`, null, false);
      }
  }