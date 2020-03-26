import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from "@angular/core/testing";
import { BrowserModule } from "@angular/platform-browser";
import { ChessGameListComponent } from "./chess-game-list.component";
import { GameListTableBodyComponent } from "./game-list-table-body/game-list-table-body.component";
import { GameListTableHeaderComponent } from "./game-list-table-header/game-list-table-header.component";
import { GameListTableFooterComponent } from "./game-list-table-footer/game-list-table-footer.component";
import { ChessGamesService } from "../services/ChessGamesService";
import { GamesListRepo } from "../repos/GamesListRepo";
import { of, throwError } from "rxjs";
import { HttpClientTestingModule } from "@angular/common/http/testing"
import { GamesListBuilder } from "../builders/GamesListBuilder";
import { GameListTableHeaderCellComponent } from
  "./game-list-table-header/game-list-table-header-cell/game-list-table-header-cell.component";
import { ChessResultPipe } from "./chess-result.pipe";

let component: ChessGameListComponent;
let fixture: ComponentFixture<ChessGameListComponent>;

let chessGameService: ChessGamesService;

describe("chess-game-list component", () => {
  beforeEach(async(() => { 
        TestBed.configureTestingModule({
          declarations: [
            ChessGameListComponent,
            GameListTableHeaderComponent,
            GameListTableHeaderCellComponent,
            GameListTableBodyComponent,
            GameListTableFooterComponent, ChessResultPipe
          ],
          imports: [
            BrowserModule,
            HttpClientTestingModule
          ],
            providers: [
              { provide: ComponentFixtureAutoDetect, useValue: true },
              ChessGamesService,
              GamesListRepo
            ]
        });
        fixture = TestBed.createComponent(ChessGameListComponent);
      component = fixture.componentInstance;
      chessGameService = TestBed.get(ChessGamesService);

    }));

    it("should loadGames", async(() => {
      spyOn(chessGameService, "loadGames").and
        .returnValue(of(new GamesListBuilder().build()));

      component.loadTable("");

      expect(component.games).not.toBeUndefined();
      expect(component.games.length).toBeGreaterThanOrEqual(1);
      expect(component.apiError).toBeFalsy();
      expect(component.errorMessage).toBeFalsy();
      expect(component.paginating).toBeFalsy();
    }));

  it("should show error", async(() => {
    const errorMessage = "500 Forced error from test";

      spyOn(chessGameService, "loadGames").and
        .returnValue(throwError(
          { message: errorMessage }));

      component.loadTable("");

      expect(component.apiError).toBeTruthy("apiError");
      expect(component.errorMessage).toBe(errorMessage);
      expect(component.games).toBeFalsy("games");
      expect(component.paginating).toBeFalsy("paginating");
    }));

});
