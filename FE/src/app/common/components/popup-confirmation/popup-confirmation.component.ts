import { Component, EventEmitter, Input, OnInit, Output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzModalModule } from 'ng-zorro-antd/modal';
@Component({
  selector: 'app-popup-confirmation',
  templateUrl: './popup-confirmation.component.html',
  standalone: true,
  imports:[NzModalModule, NzFormModule, FormsModule]
})
export class PopupConfirmationComponent implements OnInit {

  /*
   ** Indica si se muestra la confirmacion de eliminación y de que elemento
   */
   isDeleteConfirmationVisible = signal<boolean>(false);
   elementSelectedToDelete= signal<number| null>(null);
   isConfirmationvisible= signal<boolean>(false);
   elementSelected = signal<number| null>(null);
   isEmailConfirmationvisible= signal<boolean>(false);
   emailToSend!: string;

   @Input('title') title!: string;
   @Input('emailtitle') emailtitle!: string;
   @Input('header') header!: string;
   @Input('message') message!: string;
   @Input('disabled') disabled!: boolean;
   @Input('okLoading') okLoading!: boolean;
   @Output('handleOk') handleOk: EventEmitter<any> = new EventEmitter<any>();
   @Output('handleSend') handleSend: EventEmitter<string> = new EventEmitter<string>();
   @Input('emailDisabled') emailDisabled!: boolean;
  

  constructor() { }

  ngOnInit(): void {
  }

  handleCancel() {
    this.isDeleteConfirmationVisible.set(false);
    this.elementSelectedToDelete.set(null);
  }

  showDeleteConfirmation(id: number) {
    this.isDeleteConfirmationVisible.set(true);
    this.elementSelectedToDelete.set(id);
  }
  showConfirmation(){
    this.isConfirmationvisible.set(true);

  }
  handleCance() {
    this.isConfirmationvisible.set(false);
    this.elementSelectedToDelete.set(null);
  }
  
  showSendEmailConfirmation() {
    this.isEmailConfirmationvisible.set(true);
  }

  handleEmailCancel() {
    this.isEmailConfirmationvisible.set(false);
    this.emailToSend = '';
  }

  handleSendOk() {    
    this.handleSend.emit(this.emailToSend);
    this.isEmailConfirmationvisible.set(false);
  }

  isValidEmail(email: string): boolean {
    if (!email) return false;
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
  }
}
