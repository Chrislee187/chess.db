import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BaseRepo } from "./BaseRepo";

@Injectable({
  providedIn: "root"
})
export class EventsRepo extends BaseRepo {

  public static root: string = "http://localhost:5000/api/events";

  constructor(protected httpClient: HttpClient) { super(httpClient, EventsRepo.root); }

}
