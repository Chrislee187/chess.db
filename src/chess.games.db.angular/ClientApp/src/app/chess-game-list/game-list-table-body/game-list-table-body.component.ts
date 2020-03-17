import { Component, Input } from "@angular/core";
import { GamesList } from "../../repos/GamesList";

@Component({
  selector: '[game-list-table-body]',
  templateUrl: './game-list-table-body.component.html',
  styleUrls: ['./game-list-table-body.component.css']
})
export class GameListTableBodyComponent {
  @Input() list: GamesList;
  @Input() apiError: boolean;
  @Input() errorMessage: string;
}
