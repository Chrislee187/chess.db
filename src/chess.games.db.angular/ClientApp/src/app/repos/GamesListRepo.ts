import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError, of } from "rxjs";
import { ChessGameItem } from "./ChessGameItem";
import { catchError, map } from "rxjs/operators"
import { GamesList } from "./GamesList";
import { Pagination } from "./Pagination";
import { SortField } from "./SortField";

@Injectable()
export class GamesListRepo {
  constructor(private httpClient: HttpClient) { }

  public loadGames(url: string, pagination?: Pagination): Observable<GamesList> {

    if (!url) url = "http://localhost:5000/api/games";

    if (url.indexOf("?") === -1) {
      if (!pagination) {
        pagination = Pagination.default;
      }
      url += pagination.toUrlQueryParams();
    }

    console.log("Request URL: ", url);
    return this.httpClient
      .get(url,
        {
          observe: 'response',
          responseType: 'json'
        })
      .pipe(
        catchError((errorResponse: HttpErrorResponse) => {
          console.error("API Error: ", errorResponse);
          return throwError(errorResponse);;
        }),
        map((response: any) => {
          if (response.status === 200) {
            const games = response.body.value.map(d => {
              return this.mapToChessGameItem(d);
            });

            return new GamesList(games, this.getPaginationFromHeader(response));
          }

          return new GamesList();
        })
      );
  }
  
  private getPaginationFromHeader(response: any): Pagination {
    let paginationJson = response.headers.get("X-Pagination");

    return paginationJson
      ? Pagination.parseJson(paginationJson)
      : Pagination.default;
  }
  
  private mapToChessGameItem(d: any): ChessGameItem {
    return {
      white: d.White,
      black: d.Black,
      round: d.Round,
      result: d.Result,
      site: d.Site,
      event: d.Event,
      date: d.Date,
      moves: d.Moves
    };
  }
}
