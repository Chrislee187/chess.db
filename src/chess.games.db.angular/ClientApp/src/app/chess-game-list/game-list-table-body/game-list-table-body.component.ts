import { Component, OnInit, Input } from "@angular/core";
import { ChessGameItem } from "../../repos/ChessGameItem";
import { GamesList } from "../../repos/GamesList";

@Component({
  selector: '[game-list-table-body]',
  templateUrl: './game-list-table-body.component.html',
  styleUrls: ['./game-list-table-body.component.css']
})
export class GameListTableBodyComponent implements OnInit {
  @Input() list: GamesList;
  @Input() apiError: boolean;
  @Input() errorMessage: string;
  constructor() { }

  ngOnInit() {
  }

}
