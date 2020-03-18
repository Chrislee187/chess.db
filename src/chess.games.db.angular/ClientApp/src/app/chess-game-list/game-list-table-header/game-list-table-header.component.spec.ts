import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { GameListTableHeaderComponent } from "./game-list-table-header.component";
import { GameListTableHeaderCellComponent } from "./game-list-table-header-cell/game-list-table-header-cell.component";

describe("GameListTableHeaderComponent", () => {
  let component: GameListTableHeaderComponent;
  let fixture: ComponentFixture<GameListTableHeaderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [GameListTableHeaderComponent, GameListTableHeaderCellComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GameListTableHeaderComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
