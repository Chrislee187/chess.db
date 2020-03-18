import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse, HttpParams } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { ChessGameItem } from "../models/ChessGameItem";
import { catchError, map } from "rxjs/operators"
import { GamesList } from "../models/GamesList";
import { Pagination } from "../models/Pagination";
import { SortField } from "../models/SortField";

@Injectable({
  providedIn: "root"
})
export class GamesListRepo {
  constructor(private httpClient: HttpClient) { }

  loadGames(url: string, pagination?: Pagination): Observable<GamesList> {

    if (!url) url = "http://localhost:5000/api/games";

    pagination = pagination || Pagination.default;

    url = this.buildUrl(pagination, url);

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
            let sortFields = SortField.fromUrl(currentPageUrl);
            let pagination = this.getPaginationFromHeader(response, sortFields);

            let list = new GamesList(games, currentPageUrl, pagination);
            return list;
          }

          return GamesList.empty;
        })
      );
  }

    private buildUrl(pagination: Pagination, url: string) {
        if (pagination) {
            let i = url.indexOf("?");
            if (i > -1) {
                url = url.substr(0, i);
            }
            url += pagination.toUrlQueryParams();
        }
        return url;
    }

  private getPaginationFromHeader(response: any, sortFields: SortField[]): Pagination {
    const paginationJson = response.headers.get("X-Pagination");
    console.log(paginationJson);
    if (paginationJson) {
      let parsed = Pagination.parseJson(paginationJson);
      parsed.sortFields = sortFields;
      return parsed;

    }
    else
      return Pagination.default;
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
