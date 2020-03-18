import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { GameListTableHeaderCellComponent } from "./game-list-table-header-cell.component";

describe("GameListTableHeaderCellComponent", () => {
  let component: GameListTableHeaderCellComponent;
  let fixture: ComponentFixture<GameListTableHeaderCellComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GameListTableHeaderCellComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GameListTableHeaderCellComponent);
    component = fixture.componentInstance;
    component.title = "Test title";
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
