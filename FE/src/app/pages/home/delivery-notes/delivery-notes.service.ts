
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ApiService } from '../../../common/services/api.base.service';
import { DeliveryNotesModel } from './model/delivery-notes.model';

@Injectable({
    providedIn: 'root',
})
export class deliveryNotesService {
    constructor(private api: ApiService) { }
     /**
     * Obtiene un remito por Id
     * @param id
     * @returns
     */
    public getDeliveryNotesById(id: number): Observable<any> {
      return this.api.get(`deliveryNotes?id=${id}`, false)
    }
    /**
   * Obtiene todos los Comprobantes
   * @param queryParams
   * @returns
   */
  public getDeliveryNotes(queryParams: any): Observable<any> {
    return this.api.post(`deliveryNotes/list`, queryParams, false);
  }

    /**
    * Obtiene los remitos por filtro
    * @param data
    * @returns
    */
    public getByFilter(data: any): Observable<any> {
        return this.api.post(`deliveryNotes/list`, data, false);
    }
    
    /**
     * Guarda un remito
     * @param id
     * @returns
     */
     public saveDeliveryNotes(model: DeliveryNotesModel): Observable<any> {
          return this.api.post(`deliveryNotes`, model, false);
      }
      public editDeliveryNotes(model: DeliveryNotesModel): Observable<any> {
        return this.api.put(`deliveryNotes`, model, false);    
      }

      public ReprintdeliveryNotes(id: number): Observable<any> {
        return this.api.get(`Pdf/PdfRemito?id=${id}`,false , {responseType:'blob' as 'json'})
    
      }
}