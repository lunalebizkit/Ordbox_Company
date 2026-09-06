import { Component, EventEmitter, Inject, Input, LOCALE_ID, OnInit, Output } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzInputModule } from "ng-zorro-antd/input";
import { NzDatePickerModule } from "ng-zorro-antd/date-picker";
import { AppCommonModule } from "../../app.common.module";
@Component({
  selector: 'app-search-custom-filter',
  templateUrl: './search.custom.filter.component.html',
  styleUrls: ['search.custom.filter.component.css'],
  imports: [NzCollapseModule, FormsModule, NzFormModule, NzInputModule, NzDatePickerModule, ReactiveFormsModule, AppCommonModule]
})
export class SearchCustomFilterComponent implements OnInit {
  @Input() customSearchForm!: FormGroup;

  @Output('onSearchCustomClick') onSearchCustomClick: EventEmitter<any> =
    new EventEmitter<any>();

  datetime!: Date | null;

  constructor(@Inject(LOCALE_ID) public locale: string) { }

  ngOnInit(): void {
  }
  
  clearFormValue(formControl: string) {
  this.customSearchForm.get(formControl)?.setValue(null);
  this.onSearchCustomClick.emit(this.customSearchForm.value);
  }
}