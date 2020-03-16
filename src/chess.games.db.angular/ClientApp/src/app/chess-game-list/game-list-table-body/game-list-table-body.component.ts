import { Component, OnInit, Input } from "@angular/core";
import { ChessGameItem } from "../../repos/ChessGameItem";

@Component({
  selector: '[game-list-table-body]',
  templateUrl: './game-list-table-body.component.html',
  styleUrls: ['./game-list-table-body.component.css']
})
export class GameListTableBodyComponent implements OnInit {
  @Input() games: ChessGameItem[];
  @Input() apiError: boolean;
  @Input() errorMessage: string;
  constructor() { }

  ngOnInit() {
  }

}
