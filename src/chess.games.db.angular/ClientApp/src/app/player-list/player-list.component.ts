import { Component, OnInit, ViewChild } from "@angular/core";
import { PlayersRepo } from "../repos/PlayersRepo";
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

/**
 * @title Basic use of `<table mat-table>`
 */
@Component({
  selector: "player-list",
  styleUrls: ["player-list.component.css"],
  templateUrl: "player-list.component.html",
})
export class PlayerListComponent implements OnInit {

  displayedColumns: string[] = ["firstName", "middleName", "lastName"];

  public players: MatTableDataSource<Player>; // Player
  @ViewChild(MatPaginator, { static: true }) matPaginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) matSorter: MatSort;

  constructor(private playersRepo: PlayersRepo) {  }

  ngOnInit(): void {

    this.playersRepo.load().subscribe({
      next: data => {
        this.players = new MatTableDataSource<Player>(data);
        this.players.paginator = this.matPaginator;
        this.players.sort = this.matSorter;
      },
      error: error => {
        console.log("ErRoR: ", error);
      }
    });
  }
}

export interface Player {
  firstName: string;
  middleName: string;
  lastName: string;
}
