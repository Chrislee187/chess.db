import { Injectable } from "@angular/core";
import { Observable } from "rxjs"
import { GamesListRepo } from "../repos/GamesListRepo";
import { GamesList } from "../repos/GamesList";
import { Pagination as PaginationParams } from "../repos/Pagination";

@Injectable()
export class ChessGamesService {
  constructor(private gamesListRepo: GamesListRepo) { }

  loadGames(url: string, params?: PaginationParams): Observable<GamesList> {
    return this.gamesListRepo.loadGames(url, params);
  }
}
