import { Component, OnInit, Input, ElementRef } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../interceptors/auth.service';

import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { BaseComponent } from '../../components/base/base.component';
import { SecurityAuthService } from '../../../pages/auth/security-auth.service';

@Component({
  selector: 'auth-modal',
  templateUrl: './auth-modal.component.html',
  styleUrls: ['./auth-modal.component.scss'],
  imports: [FormsModule, ReactiveFormsModule, NzFormModule, NzInputModule, NzButtonModule]
})
export class AuthModalComponent extends BaseComponent implements OnInit {
  modal!: NzModalRef;
  form!: FormGroup;
  isLoading = false;

  get recaptchaControl(): AbstractControl {
    return this.form.controls['recaptcha'];
  }

  constructor(
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private fb: FormBuilder,
    private securityAuthService: SecurityAuthService,
    private authService: AuthService
  ) {
    super(notificacionService, el, message);
  }

  ngOnInit(): void {
    this.createForm();
  }

  private createForm() {
    this.form = this.fb.group(
      {
        userName: ['', [Validators.required]],
        password: ['', [Validators.required]],
        recaptcha: [''],
      },
      { updateOn: 'submit' }
    );
  }

  submit() {
    if (!this.isValidForm(this.form)) return;

    this.isLoading = true;
    const model = this.form.getRawValue();

    this.securityAuthService
      .login(model)
      .subscribe({
          next: (r) => {
            this.authService.currentUser = r;
            this.modal.triggerOk();
          },
          error: e => {
            this.showMessageError('Usuario o Contraseña invalido!!!');
          }
      })
      .add(() => {
        this.isLoading = false;
      })
  }
}
