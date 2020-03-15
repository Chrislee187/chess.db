import { Injectable } from "@angular/core";
import { Observable } from "rxjs"
import { GamesListRepo } from "../repos/GamesListRepo";
import { GamesList } from "../repos/GamesList";

@Injectable()
export class ChessGamesService {
  constructor(private gamesListRepo: GamesListRepo) { }

  loadGames(url: string): Observable<GamesList> {
    return this.gamesListRepo.loadGames(url);
  }
}
