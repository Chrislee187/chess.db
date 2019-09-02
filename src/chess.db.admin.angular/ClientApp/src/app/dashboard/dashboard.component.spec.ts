import { async, ComponentFixture, TestBed } from "@angular/core/testing";
import { HttpClientModule } from "@angular/common/http";
import { DashboardComponent } from "./dashboard.component";
import { DashletComponent } from "../dashlet/dashlet.component";


describe("DashboardComponent", () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [DashboardComponent, DashletComponent],
      imports: [HttpClientModule],
      providers: [
        {
          provide: "BASE_URL",
          useValue: "http://example.com/api",
          deps: [] 
        }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should display games dashlet", async(() => {
    expect(fixture.nativeElement
        .querySelector(".games-dashlet"))
      .not.toBeNull();
  }));

  it("should display events dashlet", async(() => {
    expect(fixture.nativeElement
        .querySelector(".events-dashlet"))
      .not.toBeNull();
  }));

  it("should display sites dashlet", async(() => {
    expect(fixture.nativeElement
        .querySelector(".sites-dashlet"))
      .not.toBeNull();
  }));

  it("should display players dashlet", async(() => {
    expect(fixture.nativeElement
        .querySelector(".players-dashlet"))
      .not.toBeNull();
  }));

});
