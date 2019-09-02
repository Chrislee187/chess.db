import { ComponentFixture } from '@angular/core/testing';
import { DashletComponent } from "./dashlet.component";

export class DashletTestComponent {
  private fixture: ComponentFixture<DashletComponent>;

  constructor(fixture: ComponentFixture<DashletComponent>) {
     this.fixture = fixture;
  }

  titleElement = () => this.fixture.nativeElement.querySelector('.title');
  valueElement = () => this.fixture.nativeElement.querySelector('.value');
  errorElement = () => this.fixture.nativeElement.querySelector('.error');
}
