import { Component, Input, Output, EventEmitter } from "@angular/core";
import { GamesList } from "../../models/GamesList";
import { SortField } from "../../models/SortField";
import { LoadDirection } from "../chess-game-list.component";

@Component({
  selector: "[game-list-table-footer]",
  templateUrl: "./game-list-table-footer.component.html",
  styleUrls: ["./game-list-table-footer.component.css"]
})
export class GameListTableFooterComponent {
  public LoadDirectionEnum = LoadDirection;

  @Output() setPageEvent = new EventEmitter<LoadDirection>();
  @Output() reSort = new EventEmitter<SortField>();

  @Input() apiError: boolean;
  @Input() paginating: boolean;
  @Input() list: GamesList;

  loadTable(direction: LoadDirection) {
    this.setPageEvent.next(direction);
  }

}
