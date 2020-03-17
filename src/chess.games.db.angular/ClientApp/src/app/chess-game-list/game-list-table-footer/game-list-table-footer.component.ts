import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { ChessGameItem } from "../../repos/ChessGameItem";
import { GamesList } from "../../repos/GamesList";

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

  loadFirst(prevUrl: string): void {
    // TODO: Fix up the Pagination meta data handling so we do this better
    let replace = prevUrl.replace(`page=${this.list.currentPage - 1}`, 'page=1');

    this.loadEvent.next(replace);
  }
  loadLast(nextUrl: string): void {
    // TODO: Fix up the Pagination meta data handling so we do this better
    let replace = nextUrl.replace(`page=${this.list.currentPage + 1}`, `page=${this.list.totalPages}`);

    this.loadEvent.next(replace);
  }
}
