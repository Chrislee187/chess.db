import { Component, OnInit } from '@angular/core';

import { ChessGameItem } from "../services/ChessGameItem";
import { ChessGameService } from "../services/ChessGameService";

@Component({
    selector: 'app-chess-game-list',
    templateUrl: './chess-game-list.component.html',
    styleUrls: ['./chess-game-list.component.css']
})

export class ChessGameListComponent implements OnInit {

  public games: ChessGameItem[];
  public previousPage: string;
  public nextPage: string;
  public firstPage: string;
  public lastPage: string;
  public currentPage: number = 1;
  public totalPages: number;

  public paginating:boolean;
      constructor(private chessGameService: ChessGameService) {

    }

    ngOnInit(): void {
      this.load("");
    }

  load(url: string) {
    console.info("Loading from: ", url);
    this.paginating = true;

    this.chessGameService.loadGames(url)
      .subscribe(success => {

        if (success) {
          this.games = this.chessGameService.games;
          this.previousPage = this.chessGameService.previousPage;
          this.nextPage = this.chessGameService.nextPage;
          this.currentPage = this.chessGameService.currentPage;
        }
        console.debug("Next: ", this.nextPage);
        console.debug("Prev: ", this.previousPage);
        this.paginating = false;
      });
  }

}

