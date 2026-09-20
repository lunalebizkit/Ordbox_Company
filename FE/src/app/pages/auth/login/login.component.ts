import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { SecurityAuthService } from '../security-auth.service';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { AuthService } from '../../../common/auth/interceptors/auth.service';
import myData from '../../../../../package.json'
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzNotificationService } from 'ng-zorro-antd/notification';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [NzButtonModule, NzFormModule, FormsModule, ReactiveFormsModule, NzInputModule, NzImageModule]
})
export class LoginComponent implements OnInit, OnDestroy {
  /**
   * Formulario
   */
  form!: FormGroup;
  /**
   * Formulario
   */
  resetPasswordForm!: FormGroup;
  /**
   * Determina si se esta guardando
   */
  isSaving= signal<boolean>(false);

  /**
   * Determina si se muestra el drawer para resetear contraseña
   */
  isResetPasswordVisible = false;

  /**
   * Constructor
   */
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private token: AuthService,
    private route: ActivatedRoute,
    private service: SecurityAuthService,
    private notification: NzNotificationService
  ) {}
  //   /**
  //    * Init event
  //    */
  ngOnInit() {
    this.form = this.fb.group({
      userName: [null, [Validators.required]],
      password: [null, [Validators.required]],
    });
  }

  getYear() {
    return new Date().getFullYear();
  }

  /**
   * Evento de login
   */
  login() {
    let model = this.getModel();
    this.isSaving.set(true);

    this.token.login(model).subscribe({
      next: (r) => {
        this.isSaving.set(false);
        this.router.navigate(['/home/products'], { relativeTo: this.route });
        this.notification.success(
          `Bienvenido ${r.userName}`,'',
          { nzPlacement: 'bottomRight' }
        );
      },
      error: () => {
        this.isSaving.set(false);
        this.notification.error('Usuario o Contraseña invalido!', '',
          { nzPlacement: 'bottomRight' }
        );
      },
    });
  }
  /**
   * Obtiene el modelo
   */
  getModel() {
    return {
      userName: this.form.controls['userName'].value,
      password: this.form.controls['password'].value,
    };
  }
  /**
   * Evento del captcha
   */
  private singleExecutionSubscription!: Subscription;
  /**
   * Evento del onDestroy
   */
  public ngOnDestroy(): void {
    if (this.singleExecutionSubscription) {
      this.singleExecutionSubscription.unsubscribe();
    }
  }
  /**
   * Evento del captcha para login
   */
  public executeImportantAction(): void {
    if (this.singleExecutionSubscription) {
      this.singleExecutionSubscription.unsubscribe();
    }
    this.login();
  }    
  
  getVersion(){
    return myData.version;
  }
}
