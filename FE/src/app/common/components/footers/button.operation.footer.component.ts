import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { NzButtonModule, NzButtonType } from "ng-zorro-antd/button";
import { NzIconModule } from "ng-zorro-antd/icon";

@Component({
    selector: 'app-button-op-footer',
    templateUrl: './button.operation.footer.component.html',
    imports: [NzIconModule, NzButtonModule],
    styleUrls: ['button.operation.footer.css']
})

export class ButtonOperationFooter implements OnInit {
    //Class
    @Input('className') className: string = '';
    //Names
    @Input('btnSaveText') btnSaveText: string = '';
    // Icons
    @Input('iconSave') iconSave!: string;
    //Events
    @Output('onSaveClick') onSaveClick: EventEmitter<any> =
        new EventEmitter<any>();
    //Spinner
    @Input('showSpinner') showSpinner!: boolean;
    //Type
    @Input('nzType') nzType: NzButtonType = 'primary';
    //Disabled
    @Input('disabled') disabled: boolean = false;
    //Red Color
    @Input('nzDanger') nzDanger: boolean = false;
    //Hidden
    @Input('hidden') hidden!: boolean;
    ngOnInit(): void { }
};