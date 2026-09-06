import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BudgetsModel } from "./model/budgets.model"; 
import { ApiService } from '../../../common/services/api.base.service';

@Injectable({
    providedIn: 'root',
})
export class BudgetsService {
  Reprintinvoice(id: number) {
    throw new Error('Method not implemented.');
  }
  
    constructor(private api: ApiService) { }
     /**
     * Obtiene una Categoria por Id
     * @param id
     * @returns
     */
    public getById(id: number): Observable<any> {
      return this.api.get(`budget/Get?id=${id}`, false)
    }

    /**
    * Obtiene los presupuestos por filtro
    * @param data
    * @returns
    */
    public getByFilter(data: any): Observable<any> {
        return this.api.post(`budget/list`, data, false);
    }
    
    /**
     * Guarda un presupuesto
     * @param id
     * @returns
     */
     public saveBudget(model: BudgetsModel): Observable<any> {
      return this.api.post(`budget/new`, model, false);    
    }

    /**
     * editar un presupuesto
     * @param id
     * @returns
     */
    public editBudget(model: BudgetsModel): Observable<any> {
      return this.api.put(`budget/edit`, model, false);    
    }



    /**
   * Obtiene un Cliente por Cuit
   * @param cuit
   * @returns
   */
  public getByCuit(cuit: string | number): Observable<any> {
    return this.api.get(`Budget/GetBudgetByCuit?cuit=${cuit}`, false);
  }
  public ReprintBudgets(id: number): Observable<any> {
    return this.api.get(`Pdf/PdfPresupuesto?id=${id}`,false, {responseType:'blob' as 'json'})

  }
   /**
     * Elimina un presupuesto por Id
     * @param id
     * @returns
     */
    public delete(id: number): Observable<any> {
      return this.api.delete(`budget/Delete?id=${id}`, false)
    }

}