import {
  Directive,
  EmbeddedViewRef,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { PermissionService } from '../auth/permission/permission-manager.service';
import { Permission } from '../auth/models/permissions.enum';

@Directive({
  selector: '[authPermission]',
})
export class PermissionDirective implements OnInit {
  private _permissionKey!: Permission[];
  private _viewRef: EmbeddedViewRef<any> | null = null;
  private _templateRef: TemplateRef<any> | null = null;

  @Input()
  set authPermission(permission: Permission[]) {
    this._permissionKey = permission;
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainerRef: ViewContainerRef,
    private permission: PermissionService
  ) {}

  ngOnInit(): void {
    setTimeout(() => {
      this.init();
    }, 0);
  }

  init() {
    const isPermitted = this.permission.validatePermissionKey(
      this._permissionKey
    );

    if (!isPermitted) return;

    this.viewContainerRef.createEmbeddedView(this.templateRef);
  }
}
