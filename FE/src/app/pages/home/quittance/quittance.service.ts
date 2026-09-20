
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ApiService } from '../../../common/services/api.base.service';
import { quittanceModel } from './model/model';


@Injectable({
    providedIn: 'root',
})
export class QuittanceService {
  
    constructor(private api: ApiService) { }
     /**
     * Obtiene un Recibo por Id
     * @param id
     * @returns
     */
    public getById(id: number): Observable<any> {
      return this.api.get(`quittance/Get?id=${id}`, false)
    }

     /**
     * Obtiene un Recibo por Id
     * @param queryParams
     * @returns
     */
  
    public getQuittance(queryParams: any): Observable<any> {
        return this.api.post(`quittance/list`, queryParams, false);
      }

    /**
    * Obtiene los recibos por filtro
    * @param data
    * @returns
    */
    public getByFilter(data: any): Observable<any> {
        return this.api.post(`quittance/list`, data, false);
    }
    
    /**
     * Guarda un recibo
     * @param id
     * @returns
     */
     public saveQuittance(model: quittanceModel): Observable<any> {
      return this.api.post(`quittance`, model, false);    
    }

    /**
     * editar un recibo
     * @param id
     * @returns
     */
    public editQuittance(model: quittanceModel): Observable<any> {
      return this.api.put(`quittance`, model, false);    
    }
    public ReprintQuittance(id: number): Observable<any> {
      return this.api.get(`Pdf/PdfRecibo?id=${id}`,false, {responseType:'blob' as 'json'})
  
    }
}