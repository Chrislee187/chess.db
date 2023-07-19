import { Component, OnInit } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { ChessGamesService as ChessGameService } from "../services/ChessGamesService";
import { ChessGameItem } from "../models/ChessGameItem";
import { GamesList } from "../models/GamesList";
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
  paginating:boolean;
  apiError: boolean;
  errorMessage: string;

  private currentUrl: string;

  constructor(private readonly chessGameService: ChessGameService) { }

  ngOnInit(): void {
    // TODO: Move to config
    const rootUrl = "http://localhost:5000/api/games";

    this.loadTable(rootUrl, Pagination.default([new SortField("White", true)]));
  }

  loadTable(url: string, pagination?: Pagination): void {
    this.chessGameService.loadGames(url, pagination)
      .subscribe({
        next: data => {
          this.currentUrl = url;

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

  setPage(direction: LoadDirection): void {
    const p = this.list.pagination;

    switch (direction) {
      case LoadDirection.FirstPage:
        p.currentPage = 1;
        break;
      case LoadDirection.PreviousPage:
        if(p.currentPage > 1) p.currentPage -= 1;
        break;
      case LoadDirection.NextPage:
        if (p.currentPage < p.totalPages) p.currentPage += 1;
        break;
      case LoadDirection.LastPage:
        p.currentPage = p.totalPages;
        break;
      default:
        console.error("!!!!!!Unknown direction!!!!!!");
        return;
    }
    this.loadTable(this.currentUrl, p);
  }

  sort(sortBy: SortField): void {
    const p = this.list.pagination;
    p.sortFields = [sortBy];
    p.currentPage = 1;
    this.loadTable(this.currentUrl, this.list.pagination);
  }

  setError(error: HttpErrorResponse | null): void {
    this.apiError = error !== null ;
    this.errorMessage = error && error.message;
  }

  get hasGames(): boolean {
    return this.list && this.list.games && this.list.games.length > 0;
  }
}

export enum LoadDirection {
  FirstPage, PreviousPage, CurrentPage, NextPage, LastPage
}

