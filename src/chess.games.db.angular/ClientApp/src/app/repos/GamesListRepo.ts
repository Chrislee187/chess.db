import { Injectable } from "@angular/core";
import { HttpClient, HttpParams, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError, of } from "rxjs";
import { ChessGameItem } from "./ChessGameItem";
import { catchError, map } from "rxjs/operators"
import { GamesList } from "./GamesList";
import { Pagination } from "./Pagination";

@Injectable()
export class GamesListRepo {
  constructor(private httpClient: HttpClient) {  }

  buildParams(paginationParams: Pagination): string {

    let urlParams = `?page-size=${paginationParams.pageSize}`;
    urlParams += `&page=${paginationParams.page}`;

    if (paginationParams.sortFields.length > 0) {
      let orderByParams = "";
      for (var field of paginationParams.sortFields) {
        orderByParams += `${field.fieldName} ${field.direction},`;
      }
      urlParams += `&order-by="${orderByParams.substr(0, orderByParams.length-1)}"`;
    }

    return urlParams;
  }

  public loadGames(url: string, pagination?: Pagination): Observable<GamesList> {

    if (url === "") url = "http://localhost:5000/api/games";

    if (url.indexOf("?") === -1) {
      if (!pagination) {
        pagination = {
          page: 1,
          pageSize: 10,
          sortFields: [{fieldName: "white", direction: "asc"}]
        };
      }

      if (pagination) {
        let urlParams = this.buildParams(pagination);
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
            const currentPage = this.getCurrentPageNumberFromUrl(url);
            const nextPage: string = this.getLink('next-page', response.body._links);
            const previousPage: string = this.getLink('prev-page', response.body._links);
            const games = response.body.value.map(d => {
              return this.mapToChessGameItem(d);
            });

            // TODO: Unpack X-Pagination header and store in GamesList
            const result = new GamesList(games, currentPage, nextPage, previousPage);
            return result;
          }

          return new GamesList([], 0, "","");
        })
    );
  }
  getCurrentPageNumberFromUrl(url: string) {
    if (url.indexOf('?') > -1) {
      const httpParams = new HttpParams({ fromString: url.split('?')[1] });
      return +httpParams.get('page');
    } else {
      return 1;
    }
  }
  getLink(name: string, links: any) {
    const t = links.find(l => l.rel === name);

    if (t) {
      return t.href;
    }

    return "";
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
