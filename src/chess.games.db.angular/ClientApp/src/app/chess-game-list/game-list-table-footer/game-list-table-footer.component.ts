import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { ChessGameItem } from "../../repos/ChessGameItem";
import { GamesList } from "../../repos/GamesList";

@Component({
  selector: '[game-list-table-footer]',
  templateUrl: './game-list-table-footer.component.html',
  styleUrls: ['./game-list-table-footer.component.css']
})
export class GameListTableFooterComponent implements OnInit {

  private _list: GamesList;
  @Output() loadEvent = new EventEmitter<string>();

  @Input() apiError: boolean;
  @Input() paginating: boolean;
  @Input() nextPage: string;
  @Input() previousPage: string;
  @Input() currentPage: number;
  @Input() set list(g: GamesList) {
    this._list = g;
  }
  get list(): GamesList {
     return this._list;
  }
  constructor() { }

  ngOnInit() {
  }

  load(url: string) {
    this.loadEvent.next(url);
  }
}
