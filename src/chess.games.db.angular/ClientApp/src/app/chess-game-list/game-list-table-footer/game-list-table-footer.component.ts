import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { ChessGameItem } from "../../models/ChessGameItem";
import { GamesList } from "../../models/GamesList";

@Component({
  selector: '[game-list-table-footer]',
  templateUrl: './game-list-table-footer.component.html',
  styleUrls: ['./game-list-table-footer.component.css']
})
export class GameListTableFooterComponent implements OnInit {

  @Output() loadEvent = new EventEmitter<string>();

  @Input() apiError: boolean;
  @Input() paginating: boolean;
  @Input() list: GamesList;

  ngOnInit() {
  }

  load(url: string): void {
    this.loadEvent.next(url);
  }
}
