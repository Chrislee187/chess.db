import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators"

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

}
