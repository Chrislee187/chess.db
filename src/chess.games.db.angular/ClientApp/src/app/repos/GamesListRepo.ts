import { Injectable } from "@angular/core";
import { HttpClient, HttpParams, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError, of } from "rxjs";
import { ChessGameItem } from "./ChessGameItem";
import { catchError, map } from "rxjs/operators"
import { GamesList } from "./GamesList";

@Injectable()
export class GamesListRepo {
  constructor(private httpClient: HttpClient) {  }

  public loadGames(url: string): Observable<GamesList> {

    if (url === "") url = "http://localhost:5000/api/games";

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
