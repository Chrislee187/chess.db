import { Component, OnInit, Input } from "@angular/core";

@Component({
  selector: '[game-list-table-header-cell]',
  templateUrl: './game-list-table-header-cell.component.html',
  styleUrls: ['./game-list-table-header-cell.component.css']
})
export class GameListTableHeaderCellComponent implements OnInit {
  @Input() title: string;
  constructor() { }

  ngOnInit() {
  }

}
