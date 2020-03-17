import { Injectable } from "@angular/core";
import { HttpClient, HttpParams, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError, of } from "rxjs";
import { ChessGameItem } from "./ChessGameItem";
import { catchError, map } from "rxjs/operators"
import { GamesList } from "./GamesList";
import { Pagination } from "./Pagination";

@Injectable()
export class GamesListRepo {
  constructor(private httpClient: HttpClient) { }

  public loadGames(url: string, pagination?: Pagination): Observable<GamesList> {

    if (!url) url = "http://localhost:5000/api/games";

    if (url.indexOf("?") === -1) {
      if (!pagination) {
        pagination = Pagination.default;
      }
    
      if (pagination) {
        let urlParams = this.buildPaginationParams(pagination);
        url += urlParams;
      }
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

            return new GamesList(games, this.getPaginationFromHeadere(response));
          }

          return new GamesList();
        })
      );
  }


  private buildPaginationParams(paginationParams: Pagination): string {

    let urlParams = `?page-size=${paginationParams.pageSize}`;
    urlParams += `&page=${paginationParams.currentPage}`;

    if (paginationParams.sortFields.length > 0) {
      let orderByParams = "";
      for (var field of paginationParams.sortFields) {
        orderByParams += `${field.fieldName} ${field.ascending ? " asc" : " desc"},`;
      }
      urlParams += `&order-by=${orderByParams.substr(0, orderByParams.length - 1)}`;
    }

    return urlParams;
  }

  private getPaginationFromHeadere(response: any): Pagination {
    let paginationJson = response.headers.get("X-Pagination");
    let pagination: Pagination;
    if (!paginationJson) {
      console.warn("No X-Pagination header found, using Default.");
      pagination = Pagination.default;
    }
    else {
      pagination = JSON.parse(paginationJson);
    }

    return pagination;
  }


  mapToChessGameItem(d: any): ChessGameItem {
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
