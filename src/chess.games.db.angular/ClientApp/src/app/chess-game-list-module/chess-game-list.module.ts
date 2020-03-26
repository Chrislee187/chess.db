import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import { ChessGameListComponent } from "./chess-game-list.component";
import { GameListTableHeaderCellComponent } from
  "./game-list-table-header/game-list-table-header-cell/game-list-table-header-cell.component";
import { GameListTableBodyComponent } from "./game-list-table-body/game-list-table-body.component";
import { GameListTableFooterComponent } from "./game-list-table-footer/game-list-table-footer.component";
import { GameListTableHeaderComponent } from "./game-list-table-header/game-list-table-header.component";

@NgModule({
  declarations: [

    ChessGameListComponent,
//    NavMenuComponent,
//    HomeComponent,
//    ,
    GameListTableHeaderComponent,
    GameListTableFooterComponent,
    GameListTableBodyComponent,
    GameListTableHeaderCellComponent
//    PlayerListComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forChild([
      { path: "chess-games", component: ChessGameListComponent }
      ])
  ],
  exports: [ChessGameListComponent],
  providers: [],
  bootstrap: [ChessGameListComponent]
})

export class ChessListModule {
  
}
