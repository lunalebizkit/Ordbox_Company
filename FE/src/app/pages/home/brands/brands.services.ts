import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BrandsModel } from "./model/brands.model";
import { ApiService } from '../../../common/services/api.base.service';

@Injectable({
    providedIn: 'root',
})
export class BrandsService {
    constructor(private api: ApiService) { }
     /**
     * Obtiene una Categoria por Id
     * @param id
     * @returns
     */
    public getById(id: number): Observable<any> {
      return this.api.get(`brand?id=${id}`, false)
    }

    /**
    * Obtiene las Categorias por filtro
    * @param data
    * @returns
    */
    public getByFilter(data: any): Observable<any> {
        return this.api.post(`brand/list`, data, false);
    }
    
    /**
     * Guarda una Categoria
     * @param id
     * @returns
     */
     public saveBrand(model: BrandsModel): Observable<any> {
        if (model.id === 0) {
          return this.api.post(`brand`, model, false);
        } else {
          return this.api.put(`brand`, model, false);
        }
      }
      /**
     * Elimina una marca por Id
     * @param id
     * @returns
     */
     public delete(id: number): Observable<any> {
      return this.api.delete(`brand/${id}`, false);
    }
}