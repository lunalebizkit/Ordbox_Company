import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { CategoryModel } from "./model/category.model";
import { ApiService } from '../../../common/services/api.base.service';

@Injectable({
    providedIn: 'root',
})
export class CategoriesService {
    constructor(private api: ApiService) { }
     /**
     * Obtiene una Categoria por Id
     * @param id
     * @returns
     */
    public getCategoryById(id: number): Observable<any> {
      return this.api.get(`category?id=${id}`, false)
    }

    /**
    * Obtiene las Categorias por filtro
    * @param data
    * @returns
    */
    public getByFilter(data: any): Observable<any> {
        return this.api.post(`category/list`, data, false);
    }
    
    /**
     * Guarda una Categoria
     * @param id
     * @returns
     */
     public saveCategory(model: CategoryModel): Observable<any> {
        if (model.id === 0) {
          return this.api.post(`category`, model, false);
        } else {
          return this.api.put(`category`, model, false);
        }
      }
        /**
     * Elimina una categoria por Id
     * @param id
     * @returns
     */
     public delete(id: number): Observable<any> {
      return this.api.delete(`category/${id}`, false);
    }
}