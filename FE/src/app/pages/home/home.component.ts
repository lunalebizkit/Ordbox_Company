import { Component, inject, OnInit, signal, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterOutlet, RouterLinkWithHref, RouterModule } from '@angular/router';
import { NzModalService, NzModalModule } from 'ng-zorro-antd/modal';
import { AuthService } from '../../common/auth/interceptors/auth.service';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzImageModule } from 'ng-zorro-antd/experimental/image';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { Permission } from '../../common/auth/models/permissions.enum';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  imports: [NzPageHeaderModule, NzIconModule, ReactiveFormsModule, NzAvatarModule, NzModalModule, RouterOutlet, NzImageModule, NzLayoutModule, NzMenuModule, RouterModule]
})
export class HomeComponent implements OnInit {
  @ViewChild('modalContent', { static: true }) modalContent!: TemplateRef<any>;
  
  usuario= signal<string | undefined>(undefined);
  permiso= signal<Permission [] | undefined>(undefined);
  color!: string;
  formModal!: FormGroup;
  
  colorList: string[] = ['#f56a00', '#7265e6', '#ffbf00', '#00a2ae', '#1112EC', '#11EC17',
'#E9EC11', '#ECA911', '#C811EC'];

  constructor(
    public token: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private modalService: NzModalService,
    private fb: FormBuilder
  ) {
    this.formModal = this.fb.group({
      number: ['', Validators.required]})
  }

  ngOnInit() {    
    this.color= this.colorList[Math.floor(Math.random() * 10)];
    this.getUser();
  }

  getYear() {
    return new Date().getFullYear();
  }
  getUser(){
    this.usuario.set(this.token.currentUser()?.userName);
    this.permiso.set(this.token.currentUser()?.permission);
  }

  logOut() {
    this.token.logout();
    this.router.navigate(['/auth'], { relativeTo: this.route });
  }

  createModal(): void {
    const modalRef = this.modalService.create({
    nzTitle: 'Abrir Whatsapp',
    nzContent: this.modalContent,
    nzClosable: false,
    nzOkDisabled: true,
    nzOnOk: () => {
      const phoneNumber = this.formModal.controls['number'].value;
      window.open(`https://wa.me/549${phoneNumber}`, '_blank');
      this.formModal.reset();
    },
    nzOnCancel: () => {
      this.formModal.reset();
    }
  });

  this.formModal.valueChanges.subscribe(() => {
    modalRef.updateConfig({
      nzOkDisabled: this.formModal.invalid || !this.formModal.get('number')?.value
    });
  });
  }
  
}
