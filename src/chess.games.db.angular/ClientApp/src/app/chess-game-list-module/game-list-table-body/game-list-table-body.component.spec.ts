import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { GameListTableBodyComponent } from "./game-list-table-body.component";
import { ChessResultPipe } from "../chess-result.pipe";

describe("GameListTableBodyComponent", () => {
  let component: GameListTableBodyComponent;
  let fixture: ComponentFixture<GameListTableBodyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GameListTableBodyComponent, ChessResultPipe ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GameListTableBodyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
