import { Component, ElementRef } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  template: '',
})
export class BaseComponent {
  /**
   * Constructor
   */
  constructor(
    private notification: NzNotificationService,
    private el: ElementRef,
    private message: NzMessageService
  ) {}

  /**
   * Show Success message
   * @param title
   * @param content
   */
  public showNotificationSuccess(title: string, content: string) {
    this.notification.create('success', title, content);
  }

  public showMessageError(message: string) {
    this.message.create('error', message);
  }

  public showMessageSuccess(message: string) {
    this.message.create('success', message);
  }
  /**
   * Determina si un form es valido
   * @param form
   * @returns
   */
  public isValidForm(
    form: FormGroup,
    showMessageError: boolean = true
  ): boolean {
    for (const key of Object.keys(form.controls)) {
      if (form.controls[key].invalid) {
        const invalidControl = this.el.nativeElement.querySelector(
          '[formcontrolname="' + key + '"]'
        );

        if (invalidControl) {
          invalidControl.focus();
        }

        this.markErrorsInControls(form);

        if (showMessageError) {
          this.showMessageError('Verifique los errores en el formulario');
        }

        return false;
      }
    }
    return true;
  }

  /**
   * Marva los errores en el formulario
   */
  markErrorsInControls(form: FormGroup) {
    for (const i in form.controls) {
      form.controls[i].markAsDirty();
      form.controls[i].updateValueAndValidity();
    }
  }

  /**
   * Convierte a base64 la imagen
   */
  getBase64(file: File): Promise<string | ArrayBuffer | null> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result);
      reader.onerror = (error) => reject(error);
    });
  }
}
