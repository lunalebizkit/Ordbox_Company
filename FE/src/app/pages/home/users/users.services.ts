
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { UserModel } from "./model/user.model";
import { ApiService } from '../../../common/services/api.base.service';

@Injectable({
    providedIn: 'root',
  })
  export class UserService {
    constructor(private api: ApiService) {}
  
    /**
     * Obtiene los usuarios por Id
     * @param id
     * @returns
     */
    public getById(id: number): Observable<any> {
      return this.api.get(`user?id=${id}`, false);
    }
  
    /**
     * Obtiene los usuarios por filtro
     * @param data
     * @returns
     */
    public getByFilter(data: any): Observable<any> {
      return this.api.post(`user/list`, data, false);
    }
    
  
    /**
     * Guarda un usuario
     * @param id
     * @returns
     */
    public saveUser(model: UserModel): Observable<any> {
      if (model.id === 0) {
        return this.api.post(`user`, model, false);
      } else {
        return this.api.put(`user`, model, false);
      }
    }
  
    /**
     * Elimina un usuario por Id
     * @param id
     * @returns
     */
     public deleteUser(id: number): Observable<any> {
      return this.api.delete(`user/${id}`, false);
    }
  
        /**
     * Obtiene los usuarios por filtro
     * @param data
     * @returns
     */
      public getPermissions(): Observable<any> {
        return this.api.post(`rol/listpermissions`, null, false);
      }
  }