import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { AddOrUpdatePermission } from "./permission/model/permission-rol.model";
import { ApiService } from "../../../common/services/api.base.service";

@Injectable({
    providedIn: 'root',
  })
  export class PermissionRolService {

constructor(public api: ApiService) {}

public permissionList(): Observable<any> {
    return this.api.post(`Rol/ListPermissions`,  false);
  }
  public permissionRolList(): Observable<any> {
    return this.api.post(`Rol/ListRolPermissions`,  false);
  };

  public addOrUpdatePermissions(model: AddOrUpdatePermission): Observable<any> {
    return this.api.post(`Rol/addorupdatepermission`, model,  false);
  }
}