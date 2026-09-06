import { Injectable } from '@angular/core';
import { ApiService } from './../../../common/services/api.base.service';
import { Observable } from 'rxjs';
import { ProductAddModel } from './model/product.add.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  /**
   * Constructor
   */
  constructor(public api: ApiService) {
  }

  /**
   * Obtiene todos los productos por query text
   * @param queryParams
   * @returns
   */
  public getProducts(queryParams: any): Observable<any> {
    return this.api.post(`product/list`, queryParams, false);
  }
  
  /**
   * Obtiene todos los productos inactivos por query text
   * @param queryParams
   * @returns
   */
  public getInactivesProducts(queryParams: any): Observable<any> {
    return this.api.post(`product/listinactive`, queryParams, false);
  }

  /**
   *Actualiza los precios de los productos 
   * @param queryParams
   * @returns
   */
  public UpdatePriceProduct(queryParams: any) : Observable<any> {
    return this.api.put(`updatepriceproduct/updatepriceproduct`, queryParams, false);
  }

  /**
   * Obtiene los productos por consulta de actualizacion
   * @param queryParams
   * @returns
   */
   public getProductsByUpdatePrice(queryParams: any) : Observable<any> {
    return this.api.post(`updatepriceproduct/list`, queryParams, false);
  }

  /**
   * Obtiene un producto por ID
   * @param id
   * @returns
   */
  public getById(id: string | number): Observable<any> {
    return this.api.get(`product?id=${id}`, false);
  }

  delete(id: string | number): Observable<any> {
    return this.api.delete(`product/${id}`, false);
  }
  activate(id: string | number): Observable<any> {
    return this.api.post(`product/activate/${id}`, false);
  }

  /**
   * Guarda un producto
   * @param model
   * @returns
   */
  public saveProduct(model: ProductAddModel): Observable<any> {
    if (model.id === 0) {
      return this.api.post(`product`, model, false);
    } else {
      return this.api.put(`product`, model, false);
    }
  }

public productsReport(){
  return this.api.post(`product/productreport`, false)
}

}

