/// <reference path="chess-game-list/chess-game-list.component.ts" />
import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { RouterModule } from "@angular/router";

import { AppComponent } from "./app.component";
import { NavMenuComponent } from "./nav-menu/nav-menu.component";
import { HomeComponent } from "./home/home.component";
import { ChessGameListComponent } from "./chess-game-list/chess-game-list.component";
import { GameListTableHeaderComponent } from "./chess-game-list/game-list-table-header/game-list-table-header.component";
import { GameListTableFooterComponent } from "./chess-game-list/game-list-table-footer/game-list-table-footer.component";
import { GameListTableBodyComponent } from "./chess-game-list/game-list-table-body/game-list-table-body.component";
import { GameListTableHeaderCellComponent } from "./chess-game-list/game-list-table-header/game-list-table-header-cell/game-list-table-header-cell.component";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ChessGameListComponent,
    GameListTableHeaderComponent,
    GameListTableFooterComponent,
    GameListTableBodyComponent,
    GameListTableHeaderCellComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: "", component: HomeComponent, pathMatch: "full", },
      { path: "chess-games", component: ChessGameListComponent},
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
