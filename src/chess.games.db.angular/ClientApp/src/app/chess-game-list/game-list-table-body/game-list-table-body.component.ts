import { Component, Input } from "@angular/core";
import { GamesList } from "../../models/GamesList";

@Component({
  selector: '[game-list-table-body]',
  templateUrl: './game-list-table-body.component.html',
  styleUrls: ['./game-list-table-body.component.css']
})
export class GameListTableBodyComponent {
  @Input() apiError: boolean;
  @Input() errorMessage: string;
  @Input() list: GamesList;

  constructor() {
     this.list = new GamesList([]);
  }
}
