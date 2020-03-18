import { Component, Input, Output, EventEmitter } from "@angular/core";
import { SortField } from "../../models/SortField";


@Component({
  selector: "[game-list-table-header]",
  templateUrl: "./game-list-table-header.component.html",
  styleUrls: ["./game-list-table-header.component.css"]
})
export class GameListTableHeaderComponent {

  @Output() reSortEvent = new EventEmitter<SortField>();
  @Input() sortFields: SortField[];

  @Input() apiError: boolean;
  
  sortBy(field: SortField) {
    this.reSortEvent.next(field);
  }


}
