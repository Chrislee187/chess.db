import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators"
import { ChessGameItem } from "../models/ChessGameItem";
import { GamesList } from "../models/GamesList";
import { Pagination } from "../models/Pagination";
import { SortField } from "../models/SortField";
import { BaseRepo } from "./BaseRepo";

@Injectable({
  providedIn: "root"
})
export class GamesListRepo extends BaseRepo{

  static rootUrl: string = "http://localhost:5000/api/games";

  constructor(protected httpClient: HttpClient) { super(httpClient, GamesListRepo.rootUrl)}

  loadGames(url: string, pagination: Pagination): Observable<GamesList> {

//    pagination = pagination || Pagination.default();

    url = this.addPaginationToUrl(pagination, url);

    return this.httpClient
      .get(url,
        {
          observe: "response",
          responseType: "json"
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
            const currentPageUrl = response.body._links[0].href;
            const sortFields = SortField.fromUrl(currentPageUrl);
            pagination = this.getPaginationFromHeader(response, sortFields);

            const list = new GamesList(games, currentPageUrl, pagination);
            return list;
          }

          return GamesList.empty;
        })
      );
  }


  private getPaginationFromHeader(response: any, sortFields: SortField[]): Pagination {
    const paginationJson = response.headers.get("X-Pagination");

    if (paginationJson) {
      const parsed = Pagination.parseJson(paginationJson);
      parsed.sortFields = sortFields;
      return parsed;
    } else {
      return Pagination.default(sortFields);
    }
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
