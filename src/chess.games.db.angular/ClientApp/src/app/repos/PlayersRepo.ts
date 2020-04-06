import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { BaseRepo } from "./BaseRepo";
import { Player } from "../player-list/player-list.component";
import { Pagination } from "../models/Pagination";
import { Observable, throwError  } from "rxjs";
import { catchError, map  } from "rxjs/operators";

@Injectable({
  providedIn: "root"
})
export class PlayersRepo extends BaseRepo {

  static rootUrl: string = "http://localhost:5000/api/players";

  constructor(protected httpClient: HttpClient) { super(httpClient, PlayersRepo.rootUrl) }

  load(pagination?: Pagination): Observable<Player[]> {
    let url = PlayersRepo.rootUrl;

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
            const players = response.body.value.map(d => {
              return this.mapToPlayer(d);
            });
            return players;
          }

          return [];
        })
      );
  }
  private mapToPlayer(d: any): Player {
    return {
      firstName: d.Firstname,
      middleName: d.MiddleNames,
      lastName: d.Lastname
    };
  }

}
