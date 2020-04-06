import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from "@angular/core";
import { SortField } from "../../../models/SortField";

@Component({
  selector: "[game-list-table-header-cell]",
  templateUrl: "./game-list-table-header-cell.component.html",
  styleUrls: ["./game-list-table-header-cell.component.css"]
})
export class GameListTableHeaderCellComponent implements OnInit, OnChanges {
  @Input() title: string;
  @Input() sortFieldName: string;
  @Input() sortFields: SortField[];
  @Input() ascending?: boolean = null;

  @Output() reSortEvent = new EventEmitter<SortField>();

  ngOnInit() {
    if (!this.sortFieldName) {
      this.sortFieldName = this.title.toLocaleLowerCase();
    }
  }

  ngOnChanges(): void {
    if (!this.sortFields) return;

    const sf = this.sortFields.filter(f => f.fieldName.toLocaleLowerCase() === this.sortFieldName.toLocaleLowerCase());

    if (sf.length > 0) {
      this.ascending = sf[0].ascending;
    } else {
      this.ascending = null;
    }
  }

  sortBy(field: string) {
    let sortAscending = this.ascending;
    if (sortAscending === null) {
      sortAscending = true;
    } else { 
      sortAscending = !sortAscending;
    }

    this.reSortEvent.next(new SortField(field,sortAscending));
  }
}
