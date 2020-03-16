import { Component, OnInit } from '@angular/core';

import { ChessGameItem } from "../repos/ChessGameItem";
import { ChessGamesService as ChessGameService } from "../services/ChessGamesService";
import { GamesList } from "../repos/GamesList";
import { HttpErrorResponse } from '@angular/common/http';

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
  public apiError: boolean;
  public errorMessage: string;
      constructor(private chessGameService: ChessGameService) {

    }

  ngOnInit(): void {
      this.load("");
    }

  load(url: string) {
    this.paginating = true;

    this.chessGameService.loadGames(url)
      .subscribe(
        (data: GamesList) => {
          if (data) {
            this.games = data.games;
            this.previousPage = data.previousPage;
            this.nextPage = data.nextPage;
            this.currentPage = data.currentPage;
          }

          this.loadFinished(null);
        },
        (error: HttpErrorResponse) => {
          this.loadFinished(error); 
        }
        );
  }

  loadFinished(error: HttpErrorResponse | null) {
    console.log("E: ", error);
    this.paginating = false;
    this.apiError = error !== null ;
    this.errorMessage = error && error.message;
  }

}


