import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzInputModule } from "ng-zorro-antd/input";

@Component({
    selector: 'app-search-filter',
    templateUrl: './search.filter.component.html',
    styleUrls: ['search.filter.component.css'],
    imports: [NzCollapseModule, NzFormModule, NzInputModule, FormsModule, CommonModule]
})
export class SearchFilterComponent implements OnInit{
    @Input('title') title!: string;
    @Input('palceHolder') palceHolder= 'Presione ENTER para busqueda';
    @Input() model: any;


    @Output('onSearchClick') onSearchClick: EventEmitter<any> =
    new EventEmitter<any>();
    @Output() modelChange = new EventEmitter<string>();
    
   constructor(){}
    ngOnInit(): void {
    }

    onModelChange(value: string) {
    this.model = value;
    this.modelChange.emit(value);
    }

    clearInput() {
    this.model = '';
    this.modelChange.emit(this.model);
    this.onSearchClick.emit();
    }
}