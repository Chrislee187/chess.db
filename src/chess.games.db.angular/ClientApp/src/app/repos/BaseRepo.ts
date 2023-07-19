import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators"
import { Pagination } from "../models/Pagination";

export class BaseRepo {

  constructor(protected httpClient: HttpClient, protected rootUrl: string) {  }

  count(): Observable<number> {
    return this.httpClient
      .get(this.rootUrl + "/count",
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
          return +response.body;
        })
      );
  }


  protected addPaginationToUrl(pagination: Pagination, url: string) {
    if (pagination) {
      const i = url.indexOf("?");
      if (i > -1) {
        url = url.substr(0, i);
      }
      url += pagination.toUrlQueryParams();
    }
    return url;
  }

}
