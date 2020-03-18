import { Component, OnInit } from '@angular/core';

import { ChessGameItem } from "../models/ChessGameItem";
import { ChessGamesService as ChessGameService } from "../services/ChessGamesService";
import { GamesList } from "../models/GamesList";
import { HttpErrorResponse } from '@angular/common/http';

@Component({
    selector: 'app-chess-game-list',
    templateUrl: './chess-game-list.component.html',
    styleUrls: ['./chess-game-list.component.css']
})

export class ChessGameListComponent implements OnInit {
  public list: GamesList;
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

  load(url: string): void {
    this.paginating = true;
    this.chessGameService.loadGames(url)
      .subscribe({
        next: data => {
          if (data) {
            this.list = data;
            this.games = data.games;
          }
          this.paginating = false;
        },
        error: error => {
          console.log("ErRoR: ", error);
          this.setError(error);
          this.paginating = false;
        }
    });
  }

  setError(error: HttpErrorResponse | null): void {
    this.apiError = error !== null ;
    this.errorMessage = error && error.message;
  }

}


