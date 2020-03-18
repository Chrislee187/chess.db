import { Component, OnInit } from "@angular/core";

import { ChessGameItem } from "../models/ChessGameItem";
import { ChessGamesService as ChessGameService } from "../services/ChessGamesService";
import { GamesList } from "../models/GamesList";
import { HttpErrorResponse } from "@angular/common/http";
import { SortField } from "../models/SortField";
import { Pagination } from "../models/Pagination";

@Component({
    selector: "app-chess-game-list",
    templateUrl: "./chess-game-list.component.html",
    styleUrls: ["./chess-game-list.component.css"]
})

export class ChessGameListComponent implements OnInit {
  list: GamesList;
  games: ChessGameItem[];
  previousPage: string;
  nextPage: string;
  firstPage: string;
  lastPage: string;
  currentPage = 1;
  totalPages: number;

  paginating:boolean;
  apiError: boolean;
  errorMessage: string;
  constructor(private readonly chessGameService: ChessGameService) {

  }

  ngOnInit(): void {
    this.load("", Pagination.default);
  }

  sort(sortBy: SortField): void {
    this.list.pagination.sortFields = [sortBy];
    this.load(this.list.currentPageUrl, this.list.pagination);
  }

  load(url: string, pagination?: Pagination): void {
    this.chessGameService.loadGames(url, pagination)
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


