import { Component, OnInit, Input } from "@angular/core";

@Component({
  selector: '[game-list-table-header]',
  templateUrl: './game-list-table-header.component.html',
  styleUrls: ['./game-list-table-header.component.css']
})
export class GameListTableHeaderComponent {

  @Input() apiError:boolean;
}
