import { async, ComponentFixture, TestBed } from "@angular/core/testing";
import { HttpClientModule } from "@angular/common/http";
import { DashletComponent } from "./dashlet.component";
import { DashletTestComponent } from "./dashlet-test-component";


describe('DashletComponent', () => {
  let component: DashletComponent;
  let fixture: ComponentFixture<DashletComponent>;
  let dashlet: DashletTestComponent;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [DashletComponent],
      imports: [HttpClientModule],
      providers: [
        {
          provide: 'BASE_URL',
          useValue: "http://example.com/api",
          deps: [] 
        }
      ]
    })
      .compileComponents();
  }));

  const dashletTitle: string = 'test-dashlet';

  beforeEach(() => {
    fixture = TestBed.createComponent(DashletComponent);
    dashlet = new DashletTestComponent(fixture);
    component = fixture.componentInstance;
    component.name = dashletTitle;
    fixture.detectChanges();
  });

  it('should display dashlet title', async(() => {
    expect(dashlet.titleElement().textContent)
      .toContain(dashletTitle);
  }));

  it('should display \'counting\' until value is received', async(() => {
    expect(dashlet.valueElement().textContent)
      .toContain('Counting');

    component.totalCount = 10;
    fixture.detectChanges();

    expect(dashlet.valueElement().textContent)
      .toContain('10');
  }));

  it('should display error and title only when error', async(() => {
    expect(dashlet.errorElement())
      .toBeNull();
    expect(dashlet.valueElement())
      .not.toBeNull();

    component.error = "*error*";
    fixture.detectChanges();

    expect(dashlet.errorElement().textContent)
      .toContain("*error*");
    expect(dashlet.titleElement().textContent)
      .toContain(dashletTitle);
    expect(dashlet.valueElement())
      .toBeNull();
  }));
});
