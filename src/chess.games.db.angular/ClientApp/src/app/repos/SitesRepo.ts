import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BaseRepo } from "./BaseRepo";

@Injectable({
  providedIn: "root"
})
export class SitesRepo extends BaseRepo {

  public static root: string = "http://localhost:5000/api/sites";

  constructor(protected httpClient: HttpClient) { super(httpClient, SitesRepo.root); }

}
