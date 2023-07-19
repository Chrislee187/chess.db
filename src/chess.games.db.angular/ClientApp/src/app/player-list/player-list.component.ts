import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { PlayersRepo } from "../repos/PlayersRepo";
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { PlayersDataSource } from "./PlayersDataSource";
import { Pagination } from "../models/Pagination";
import { tap } from "rxjs/operators";
import { merge } from "rxjs";
import { SortField } from "../models/SortField";

@Component({
  selector: "player-list",
  styleUrls: ["player-list.component.css"],
  templateUrl: "player-list.component.html",
})
export class PlayerListComponent implements OnInit, AfterViewInit {
  private pagination: Pagination = Pagination.default([new SortField("lastName", true)]);

  displayedColumns: string[] = ["firstName", "middleName", "lastName"];

  dataSource: PlayersDataSource;

  pageSize: number = this.pagination.pageSize;

  @ViewChild(MatPaginator, { static: true }) matPaginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) matSorter: MatSort;

  constructor(private playersRepo: PlayersRepo) {
  }


  ngOnInit(): void {
    this.dataSource = new PlayersDataSource(this.playersRepo);
    this.dataSource.loadPlayers(this.pagination);
  }

  ngAfterViewInit() {
   this.matSorter.sortChange.subscribe(() => this.matPaginator.pageIndex = 0);

    merge(this.matSorter.sortChange, this.matPaginator.page)
      .pipe(
        tap(() => this.loadPlayersPage())
      )
      .subscribe();
  }

  loadPlayersPage() {
    this.pagination.pageSize = this.matPaginator.pageSize;
    this.pagination.currentPage = this.matPaginator.pageIndex + 1;

    this.pagination.sortFields = [new SortField(this.matSorter.active, this.matSorter.direction === 'asc')];

    this.dataSource.loadPlayers(this.pagination);
  }
}


export interface Player {
  firstName: string;
  middleName: string;
  lastName: string;
}
