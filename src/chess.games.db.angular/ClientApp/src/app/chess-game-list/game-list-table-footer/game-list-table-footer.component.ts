import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { GamesList } from "../../models/GamesList";
import { SortField } from "../../models/SortField";

@Component({
  selector: "[game-list-table-footer]",
  templateUrl: "./game-list-table-footer.component.html",
  styleUrls: ["./game-list-table-footer.component.css"]
})
export class GameListTableFooterComponent implements OnInit {

  @Output() loadEvent = new EventEmitter<string>();
  @Output() reSort = new EventEmitter<SortField>();

  @Input() apiError: boolean;
  @Input() paginating: boolean;
  @Input() list: GamesList;

  ngOnInit() {
  }

  load(url: string): void {
    this.loadEvent.next(url);
  }
}
