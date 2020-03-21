import { Injectable } from "@angular/core";
import { Observable, forkJoin, } from "rxjs"
import { map } from "rxjs/operators"
import { GamesListRepo } from "../repos/GamesListRepo";
import { DashboardSummary } from "../models/DashboardSummary";
import { PlayersRepo } from "../repos/PlayersRepo";
import { EventsRepo } from "../repos/EventsRepo";
import { SitesRepo } from "../repos/SitesRepo";

@Injectable({
  providedIn: "root"
})
export class DashboardService {
  constructor(
    private gamesListRepo: GamesListRepo,
    private playersRepo: PlayersRepo,
    private eventsRepo: EventsRepo,
    private sitesRepo: SitesRepo) { }

  dashboardSummary(): Observable<DashboardSummary> {

    const getGames = this.gamesListRepo.count();
    const getPlayers = this.playersRepo.count();
    const getEvents = this.eventsRepo.count();
    const getSites = this.sitesRepo.count();


    return forkJoin([getGames, getPlayers, getEvents, getSites])
      .pipe(map(results => {
        return {
          gamesCount: results[0],
          playersCount: results[1],
          eventsCount: results[2],
          sitesCount: results[3]
        } as DashboardSummary;
    }));
  }
}
