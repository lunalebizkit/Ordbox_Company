import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { PermissionDirective } from '../../directives/permission.directive';
import { Permission } from '../../auth/models/permissions.enum';

@Component({
  selector: 'app-header-op-buttons',
  templateUrl: './buttons.oparations.header.component.html',
  styleUrls: ['button.operations.header.css'],
  imports:[NzPageHeaderModule, NzIconModule, NzTagModule, NzButtonModule, PermissionDirective]
})

export class HeaderOperationsButtonsComponent implements OnInit {
  permissions = Permission;
  
  @Input('btnSaveText') btnSaveText: string = 'Guardar';
  @Input('btnSendText') btnSendText: string = 'Guardar y Enviar';
  @Input('btnSendEmail') btnSendEmail?: string = 'Enviar';
  @Input('btnCancelText') btnCancelText: string = 'Volver';
  @Input('btnCloseText') btnCloseText: string = 'Volver';
  @Input('btnDeleteText') btnDeleteText: string = 'Eliminar';
  @Input('btnUpdateText') btnUpdateText: string = 'Actualizar';
  @Input('btnReprintText') btnReprintText: string = 'Reimprimir';
  @Input('btnPrintText') btnPrintText: string = 'Imprimir';
  @Input('btnCerrarText') btnCerrarText: string = 'Cerrar';
  @Input('btnFacturaProText') btnFacturaProText: string = 'Factura Proforma';
  @Input('btnFacturaARCAText') btnFacturaARCAText: string = 'Factura ARCA';
  @Input('btnEnviarFacturaARCAText') btnEnviarFacturaARCAText: string = 'Enviar Factura';
  @Input('btnEditText') btnEditText: string = '';
  @Input() editPermission: any;

  @Input('tagText') tagText: string = '';
  @Input('title') title!: string;
  @Input('color') color!: string;
  @Input('iconSave') iconSave!: string;
  @Input('iconDelete') iconDelete!: string;
  @Input('iconTitle') iconTitle!: string;
  @Input('iconBack') iconBack!: string;
  @Input('iconSend') iconSend!: string;
  @Input('iconUpdate') iconUpdate!: string;
  @Input('colors') colors!: string;
  

  @Input('iconReprint') iconReprint!: string;
  @Input('iconPrint') iconPrint!: string;
  @Input('iconFacturaPro') iconFacturaPro!: string;
  @Input('iconFacturaARCA') iconFacturaARCA!: string;
  @Input('iconEnviarFacturaARCA') iconEnviarFacturaARCA!: string;


  @Input('showSpinner') showSpinner!: boolean;
  @Input('showTag') showTag: boolean = false;
  @Input('showUpdate') showUpdate: boolean = false;
  @Input('showSave') showSave: boolean = true;
  @Input('showBack') showBack: boolean = false;
  @Input('showClose') showClose: boolean = false;
  @Input('showDelete') showDelete: boolean = false;
  @Input('showSend') showSend: boolean = false;

  @Input('showReprint') showReprint: boolean = false;
  @Input('showPrint') showPrint: boolean = false;
  @Input('showCerrar') showCerrar: boolean = false;
  @Input('showFacturaPro') showFacturaPro: boolean = false;
  @Input('showARCA') showARCA: boolean = false;


  @Input('showEmail') showEmail: boolean = false;
  @Input('disabled') disabled: boolean = false;
  @Input('disabledARCA') disabledARCA: boolean = false;
  @Input('disabledEnviarARCA') disabledEnviarARCA: boolean = false;
  @Input('disabledEdit') disabledEdit: boolean = false;

  @Output('onSaveClick') onSaveClick: EventEmitter<any> =
    new EventEmitter<any>();
  @Output('onSendClick') onSendClick: EventEmitter<any> =
    new EventEmitter<any>();
  @Output('onSendEmailClick') onSendEmailClick: EventEmitter<any> =
    new EventEmitter<any>();
  @Output('onCancelClick') onCancelClick: EventEmitter<any> =
    new EventEmitter<any>();
  @Output('onDeleteClick') onDeleteClick: EventEmitter<any> =
    new EventEmitter<any>();
  @Output('onCloseClick') onCloseClick: EventEmitter<any> =
    new EventEmitter<any>();
  @Output('onUpdateClick') onUpdateClick: EventEmitter<any> =
    new EventEmitter<any>();
  @Output('onReprintClick') onReprintClick: EventEmitter<any> =
    new EventEmitter<any>();  

    @Output('onPrintClick') onPrintClick: EventEmitter<any> =
    new EventEmitter<any>();  
    @Output('onCerrarClick') onCerrarClick: EventEmitter<any> =
    new EventEmitter<any>();  
    @Output('onFacturaProClick') onFacturaProClick: EventEmitter<any> =
    new EventEmitter<any>();  
    @Output('onFacturaARCAClick') onFacturaARCAClick: EventEmitter<any> =
    new EventEmitter<any>();  
    @Output('onEnviarEmailFacturaARCAClick') onEnviarEmailFacturaARCAClick: EventEmitter<any> =
    new EventEmitter<any>();  
    @Output('onEditClick') onEditClick: EventEmitter<any>= new EventEmitter();

  constructor(private route: ActivatedRoute, private router: Router) { }

  ngOnInit() { }

  goBack() {
    if (this.onCancelClick.length > 0) {
      this.onCancelClick.emit();
    } else {
      this.route.params.subscribe((p) => {
        let url = this.router.routerState.snapshot.url
          .split('home/', 2)[1]
          .split('/')[0];
        switch (url) {
          case 'permission':
            this.router.navigate(['/home/products']);
            break;

          default:
            if (p['id'] || p['invoiceId'] || p['creditId'] || p['debitId']) {
              this.router.navigate(['../../'], { relativeTo: this.route });
            } else {
              this.router.navigate(['../'], { relativeTo: this.route });
            }
            break;
        }
      });
    }
  }

}
function Directive(arg0: { selector: string; }): (target: typeof HeaderOperationsButtonsComponent) => void | typeof HeaderOperationsButtonsComponent {
  throw new Error('Function not implemented.');
}

