
import {CollectionViewer, DataSource} from "@angular/cdk/collections";
import { Player, Player as IPlayer } from "./player-list.component";
import { BehaviorSubject, Observable, of, merge } from "rxjs";
import { PlayersRepo } from "../repos/PlayersRepo";
import { Pagination } from "../models/Pagination";

import { SortField } from "../models/SortField";

export class PlayersDataSource implements DataSource<Player> {
  private playersSubject = new BehaviorSubject<Player[]>([]);
  private loadingSubject =new BehaviorSubject<boolean>(false);
  private countSubject =new BehaviorSubject<number>(1);

  public loading$ = this.loadingSubject.asObservable();
  public playersCount$ = this.countSubject.asObservable();
  constructor(private playerRepo: PlayersRepo) { }

  connect(collectionViewer: CollectionViewer): Observable<Player[]> {
    return this.playersSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.playersSubject.complete();  
    this.loadingSubject.complete();  
    this.countSubject.complete();  
  }

  
  loadPlayers(pagination: Pagination) {
    this.loadingSubject.next(true);

    merge(this.playerRepo.count(), this.playerRepo.load(pagination))
      .subscribe(d => console.log("D:", d));

    this.playerRepo.count().subscribe(v => {
      this.countSubject.next(v);
    });

    this.playerRepo.load(pagination)
        .pipe(
          catchError(() => of([])),
          finalize(() => {
            this.loadingSubject.next(false);
          })
        )
        .subscribe(data => {
          this.playersSubject.next(data);
        })
      ;
  }  
}

import { catchError, finalize } from "rxjs/operators";
